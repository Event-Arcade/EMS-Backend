﻿using Amazon.S3;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.Account;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Enums;
using EMS.BACKEND.API.Mappers;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Newtonsoft.Json;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class AccountRepository : IUserAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICloudProviderRepository _cloudProvider;
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITokenService _tokenService;
        private readonly INotificationRepository _notificationRepository;

        public AccountRepository(ITokenService tokenService, UserManager<ApplicationUser> userManager, INotificationRepository notificationRepository,
                                 ICloudProviderRepository cloudProvider, IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _userManager = userManager;
            _cloudProvider = cloudProvider;
            _config = config;
            _scopeFactory = scopeFactory;
            _tokenService = tokenService;
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponseDTO<string>> CreateAccountAsync(RegisterUserDTO registerUser)
        {
            try
            {
                //check weather user already registered
                var user = await _userManager.FindByEmailAsync(registerUser.Email);
                if (user is not null)
                    return new BaseResponseDTO<string>
                    {
                        Flag = false,
                        Message = "User already registered"
                    };

                //Map DTO to ApplicationUser
                var registerApplicationUser = registerUser.MapRegisterUserDTOToApplicationUser();

                //upload profile picture to storage
                if (registerUser.ProfilePicture != null)
                {
                    var (condition, filepath) = await _cloudProvider.UploadFile(registerUser.ProfilePicture, _config["StorageDirectories:ProfileImages"]);
                    if (condition)
                    {
                        registerApplicationUser.ProfilePicturePath = filepath;
                    }
                    else
                    {
                        return new BaseResponseDTO<string>
                        {
                            Flag = false,
                            Message = filepath
                        };
                    }
                }
                else
                {
                    // Assign default image path
                    registerApplicationUser.ProfilePicturePath = _config["StorageDirectories:ProfileImages"] + "/default.png";
                }

                //Create user
                var createUser = await _userManager.CreateAsync(registerApplicationUser, registerUser.Password);
                //Check user created
                if (!createUser.Succeeded)
                    return new BaseResponseDTO<string>
                    {
                        Flag = false,
                        Message = createUser.ToString()
                    };

                //Assign Default Role : "client"
                await _userManager.AddToRoleAsync(registerApplicationUser, "client");


                //generate jwt token
                var getUser = await _userManager.FindByEmailAsync(registerUser.Email);
                if (getUser is not null)
                {
                    var getUserRole = await _userManager.GetRolesAsync(getUser);
                    var token = _tokenService.CreateToken(getUser, getUserRole.First());

                    // send notification to admin
                    //TODO: fix the string values as parameter for the notification error or remove  the notification
                    //await _notificationRepository.AddNotification("New User Registered", "A new user has registered", DatabaseChangeEventType.Add, "admin", null, EntityType.User, int.Parse(getUser.Id), getUser.Id);
                    await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Add, EntityType.User, getUser.Id, getUser.Id);

                    return new BaseResponseDTO<string>
                    {
                        Flag = true,
                        Message = "Account created",
                        Data = token
                    };
                }

                return new BaseResponseDTO<string>
                {
                    Flag = false,
                    Message = "Error occured while creating account"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<string>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<BaseResponseDTO<string>> LoginAccountAsync(LoginDTO loginDTO)
        {
            try
            {
                var getUser = await _userManager.FindByEmailAsync(loginDTO.Email);

                //Check user is not null
                if (getUser is null)
                    return new BaseResponseDTO<string>
                    {
                        Flag = false,
                        Message = "User not found"
                    };

                //Check user password is correct
                bool checkUserPasswords = await _userManager.CheckPasswordAsync(getUser, loginDTO.Password);
                if (!checkUserPasswords)
                    return new BaseResponseDTO<string>
                    {
                        Flag = false,
                        Message = "Incorrect password/username"
                    };

                //generate jwt token
                var getUserRole = await _userManager.GetRolesAsync(getUser);
                var token = _tokenService.CreateToken(getUser, getUserRole.First());

                // Assign PresignedURL for profilepicture path
                if (getUser.ProfilePicturePath != null)
                {
                    var url = _cloudProvider.GeneratePreSignedUrlForDownload(getUser.ProfilePicturePath);
                    getUser.ProfilePicturePath = url;
                }

                return new BaseResponseDTO<string>
                {
                    Flag = true,
                    Message = "Login successful",
                    Data = token
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<string>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<BaseResponseDTO<UserAccountResponseDTO>> UpdateAccountAsync(string userId, UpdateUserDTO userDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                //Check user is not null
                if (user is null)
                    return new BaseResponseDTO<UserAccountResponseDTO>
                    {
                        Flag = false,
                        Message = "User not found"
                    };

                //Store updated image if new image is uploaded
                if (userDTO.ProfilePicture != null)
                {
                    var (condition, filepath) = await _cloudProvider.UploadFile(userDTO.ProfilePicture, _config["StorageDirectories:ProfileImages"]);
                    if (condition)
                    {
                        //remove previous image from storage if not default image
                        if (user.ProfilePicturePath != null && user.ProfilePicturePath != _config["StorageDirectories:ProfileImages"] + "/default.png")
                        {
                            await _cloudProvider.RemoveFile(user.ProfilePicturePath);
                        }
                        //assign new image
                        user.ProfilePicturePath = filepath;
                    }
                    else
                    {
                        return new BaseResponseDTO<UserAccountResponseDTO>
                        {
                            Flag = false,
                            Message = filepath
                        };
                    }
                }

                // Update user details
                if (userDTO.FirstName != null)
                    user.FirstName = userDTO.FirstName;
                if (userDTO.LastName != null)
                    user.LastName = userDTO.LastName;
                if (userDTO.Street != null)
                    user.Street = userDTO.Street;
                if (userDTO.City != null)
                    user.City = userDTO.City;
                if (userDTO.PostalCode != null)
                    user.PostalCode = userDTO.PostalCode;
                if (userDTO.Province != null)
                    user.Province = userDTO.Province;
                if (userDTO.Longitude != null)
                    user.Longitude = (double)userDTO.Longitude;
                if (userDTO.Latitude != null)
                    user.Latitude = (double)userDTO.Latitude;



                //Update user
                var updatedUser = await _userManager.UpdateAsync(user);
                if (!updatedUser.Succeeded)
                {

                    return new BaseResponseDTO<UserAccountResponseDTO>
                    {
                        Flag = false,
                        Message = updatedUser.ToString(),
                    };
                }
                else
                {
                    //Get user Profile Picture URL
                    if (user.ProfilePicturePath != null)
                    {
                        var url = _cloudProvider.GeneratePreSignedUrlForDownload(user.ProfilePicturePath);
                        user.ProfilePicturePath = url;
                    }
                    // get the role of the user
                    var userRole = await _userManager.GetRolesAsync(user);

                    // send databaseChange to admins, vendors, and clients
                    await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Update, EntityType.User, user.Id, user.Id);

                    return new BaseResponseDTO<UserAccountResponseDTO>
                    {
                        Flag = true,
                        Message = "User updated",
                        Data = user.MapUserToUserAccountResponseDTO(userRole.First().ToString())
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<UserAccountResponseDTO>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }

        }
        public async Task<BaseResponseDTO<UserAccountResponseDTO>> GetAccountByIdAsync(string userId)
        {
            try
            {//Get user
                var currentUser = await _userManager.FindByIdAsync(userId);

                //Check user if exist
                if (currentUser != null)
                {
                    //Create user response object using Mapper
                    //Get user Profile Picture URL
                    if (currentUser.ProfilePicturePath != null)
                    {
                        var url = _cloudProvider.GeneratePreSignedUrlForDownload(currentUser.ProfilePicturePath);
                        currentUser.ProfilePicturePath = url;
                    }
                    // get the role of the user
                    var userRole = await _userManager.GetRolesAsync(currentUser);

                    return new BaseResponseDTO<UserAccountResponseDTO>
                    {
                        Flag = true,
                        Message = "User found",
                        Data = currentUser.MapUserToUserAccountResponseDTO(userRole.First().ToString())

                    };
                }

                return new BaseResponseDTO<UserAccountResponseDTO>
                {
                    Flag = false,
                    Message = "User not found"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<UserAccountResponseDTO>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }

        }
        public async Task<BaseResponseDTO> DeleteAccountAsync(string userId)
        {
            try
            {            //Get user by id
                var user = await _userManager.FindByIdAsync(userId);

                //Check user is not null
                if (user is null)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "User not found"
                    };
                }

                // Check if user is an admin
                if (await _userManager.IsInRoleAsync(user, "admin"))
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "This is an admin account, therefore cannot delete account"
                    };
                }

                // Check if user has any shops
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Check if user has any shops
                    var shop = await context.Shops.FirstOrDefaultAsync(x => x.OwnerId == userId);
                    if (shop != null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "User has/have registed shops, therefore cannot delete account"
                        };
                    }

                    // Check if user has any packages user has ordered
                    var package = await context.Packages.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                    if (package != null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "User has/have ordered service(s), therefore cannot delete account"
                        };
                    }

                    /*
                    when a user is deleted, all the messages sent by the user should be update to 
                    show that the user has been deleted
                    */
                    var messages = await context.ChatMessages.Where(x => x.SenderId == userId).ToListAsync();
                    if (messages.Count > 0)
                    {
                        foreach (var message in messages)
                        {
                            // update the message
                            message.Message = "This user has been deleted";

                            context.ChatMessages.Update(message);
                        }
                        await context.SaveChangesAsync();
                    }
                }

                // Remove user profile picture from storage
                if (user.ProfilePicturePath != null && user.ProfilePicturePath != _config["StorageDirectories:ProfileImages"] + "/default.png")
                {
                    await _cloudProvider.RemoveFile(user.ProfilePicturePath);
                }

                // Delete user
                var deletedUser = await _userManager.DeleteAsync(user);
                if (!deletedUser.Succeeded)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = deletedUser.ToString()
                    };
                }

                // send notification to admin
                //TODO: fix the string values as parameter for the notification error or remove  the notification
                //await _notificationRepository.AddNotification("User Deleted", "A user has been deleted", DatabaseChangeEventType.Delete, "admin", null, EntityType.User, int.Parse(user.Id), user.Id);

                // send database change event to all users
                await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Delete, EntityType.User, user.Id, user.Id);

                return new BaseResponseDTO
                {
                    Flag = true,
                    Message = "Account deleted"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<BaseResponseDTO> UpdateAccountPasswordAsync(string userId, UpdatePasswordDTO updatePasswordDTO)
        {
            try
            {            // Check old password  and new password is empty or same
                if (string.IsNullOrEmpty(updatePasswordDTO.OldPassword) || string.IsNullOrEmpty(updatePasswordDTO.NewPassword) || updatePasswordDTO.OldPassword == updatePasswordDTO.NewPassword)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "Invalid password"
                    };
                }

                //Get user by id
                var user = _userManager.FindByIdAsync(userId).Result;
                if (user == null)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "User not found"
                    };
                }

                //Check old password is correct
                var checkPassword = await _userManager.CheckPasswordAsync(user, updatePasswordDTO.OldPassword);
                if (!checkPassword)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "Incorrect password"
                    };
                }

                //Change password
                var changePassword = await _userManager.ChangePasswordAsync(user, updatePasswordDTO.OldPassword, updatePasswordDTO.NewPassword);
                if (!changePassword.Succeeded)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = changePassword.ToString()
                    };
                }

                return new BaseResponseDTO
                {
                    Flag = true,
                    Message = "Password updated"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<BaseResponseDTO<string>> GoogleLoginAsync(GoogleLoginDTO googleLoginDTO)
        {
            try
            {
                var token = googleLoginDTO.Token;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=" + token);
                var response = await client.SendAsync(request);
                var payload = await response.Content.ReadAsStringAsync();
                var googleUser = JsonConvert.DeserializeObject<GoogleUser>(payload);

                if (googleUser is null)
                {
                    return new BaseResponseDTO<string>
                    {
                        Flag = false,
                        Message = "Invalid google token"
                    };
                }

                var user = await _userManager.FindByEmailAsync(googleUser.Email);
                if (user is null)
                {
                    var newUser = new ApplicationUser
                    {
                        Email = googleUser.Email,
                        UserName = googleUser.Email,
                        FirstName = googleUser.GivenName,
                        LastName = googleUser.FamilyName,
                        EmailConfirmed = true,
                        ProfilePicturePath = googleUser.Picture
                    };

                    var createUser = await _userManager.CreateAsync(newUser);
                    if (!createUser.Succeeded)
                    {
                        return new BaseResponseDTO<string>
                        {
                            Flag = false,
                            Message = createUser.ToString()
                        };
                    }

                    await _userManager.AddToRoleAsync(newUser, "client");
                    user = newUser;
                }

                var userRole = await _userManager.GetRolesAsync(user);
                var jwtToken = _tokenService.CreateToken(user, userRole.First());

                return new BaseResponseDTO<string>
                {
                    Flag = true,
                    Message = "Login successful",
                    Data = jwtToken
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<string>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<BaseResponseDTO<ICollection<UserAccountResponseDTO>>> GetAllUsersAsync(string userId)
        {
            try
            {
                // check if the user is an admin
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new BaseResponseDTO<ICollection<UserAccountResponseDTO>>
                    {
                        Flag = false,
                        Message = "User not found"
                    };
                }

                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    return new BaseResponseDTO<ICollection<UserAccountResponseDTO>>
                    {
                        Flag = false,
                        Message = "You are not authorized to view this page"
                    };
                }

                // get all users except the current user
                var users = await _userManager.Users.Where(x => x.Id != userId).ToListAsync();
                if (users.Count == 0)
                {
                    return new BaseResponseDTO<ICollection<UserAccountResponseDTO>>
                    {
                        Flag = false,
                        Message = "No users found"
                    };
                }

                // set the user profile picture URL
                foreach (var u in users)
                {
                    if (u.ProfilePicturePath != null)
                    {
                        var url = _cloudProvider.GeneratePreSignedUrlForDownload(u.ProfilePicturePath);
                        u.ProfilePicturePath = url;
                    }
                }

                List<UserAccountResponseDTO> userAccountResponseDTOs = new List<UserAccountResponseDTO>();

                foreach (var us in users)
                {
                    var userRole = await _userManager.GetRolesAsync(us);
                    userAccountResponseDTOs.Add(us.MapUserToUserAccountResponseDTO(userRole.First().ToString()));
                }

                return new BaseResponseDTO<ICollection<UserAccountResponseDTO>>
                {
                    Flag = true,
                    Message = "Users found",
                    Data = userAccountResponseDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ICollection<UserAccountResponseDTO>>
                {
                    Flag = false,
                    Message = ex.Message
                };

            }
        }
        public async Task UpdateUserRoleAsync(string userId, UpdateUserRoleDTO updateUserRoleDTO)
        {
            try
            {
                // check if the user is an admin
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    throw new Exception("You are not authorized");
                }

                // get the user
                var userToUpdate = await _userManager.FindByIdAsync(updateUserRoleDTO.UserId);
                if (userToUpdate == null)
                {
                    throw new Exception("User not found");
                }

                // check if the user is an admin, if it is, remove the admin role and assign the client role
                if (await _userManager.IsInRoleAsync(userToUpdate, "admin"))
                {
                    await _userManager.RemoveFromRoleAsync(userToUpdate, "admin");
                    await _userManager.AddToRoleAsync(userToUpdate, "client");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToUpdate, "client");
                    await _userManager.AddToRoleAsync(userToUpdate, "admin");
                }

                //TODO: fix the string values as parameter for the notification error or remove  the notification
                // send notification to the user
                //await _notificationRepository.AddNotification("Role Updated", "Your role has been updated", DatabaseChangeEventType.Update, userToUpdate.Id, null, EntityType.User, int.Parse(userToUpdate.Id), userToUpdate.Id);

                // send database change event to all users
                await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Update, EntityType.User, userToUpdate.Id, userToUpdate.Id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
