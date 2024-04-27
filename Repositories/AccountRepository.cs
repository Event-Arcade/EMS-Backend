using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedClassLibrary.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EMS.BACKEND.API.Repositories
{
    public class AccountRepository(UserManager<ApplicationUser> userManager,
        IConfiguration config, IHttpContextAccessor httpContextAccessor, ICloudProviderRepository cloudProvider, IServiceScopeFactory scopeFactory) : IUserAccountRepository
    {
        public async Task<BaseResponseDTO<String>> CreateAccount(ApplicationUser applicationUser)
        {
            //Check model is empty
            if (applicationUser is null) return new BaseResponseDTO<String>
            {
                Flag = false,
                Message = "Model is empty"
            };

            //check weather user already registered
            var user = await userManager.FindByEmailAsync(applicationUser.Email);
            if (user is not null)
                return new BaseResponseDTO<String>
                {
                    Flag = false,
                    Message = "User already registered"
                };

            //create new user object
            var newUser = new ApplicationUser()
            {
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                Email = applicationUser.Email,
                UserName = applicationUser.Email,
                Street = applicationUser.Street,
                City = applicationUser.City,
                PostalCode = applicationUser.PostalCode,
                Province = applicationUser.Province,
                Longitude = applicationUser.Longitude,
                Latitude = applicationUser.Latitude,
            };

            //store profile-picture in storage
            var (condition, filepath) = await cloudProvider.UploadFile(applicationUser.ProfilePicture, config["StorageDirectories:ProfileImages"]);
            if (condition)
            {
                newUser.ProfilePicturePath = filepath;
            }
            else
            {
                return new BaseResponseDTO<String>
                {
                    Flag = false,
                    Message = filepath
                };
            }

            var createUser = await userManager.CreateAsync(newUser, applicationUser.Password);
            //Check user created
            if (!createUser.Succeeded)
                return new BaseResponseDTO<String>
                {
                    Flag = false,
                    Message = createUser.ToString()
                };

            //Assign Default Role : "client"
            await userManager.AddToRoleAsync(newUser, "client");


            //generate jwt token
            var getUser = await userManager.FindByEmailAsync(newUser.Email);
            if (getUser is not null)
            {
                var getUserRole = await userManager.GetRolesAsync(getUser);
                var userSession = new UserSession()
                {
                    Id = getUser.Id,
                    Email = getUser.Email,
                    Role = getUserRole.First()
                };
                string token = GenerateToken(userSession);
                return new BaseResponseDTO<string>
                {
                    Flag = true,
                    Message = "Account Created",
                    Data = token
                };
            }

            return new BaseResponseDTO<String>
            {
                Flag = false,
                Message = "Error occured while creating account"
            };
        }
        public async Task<BaseResponseDTO<String>> LoginAccount(ApplicationUser loginDTO)
        {
            //Check login container is empty
            if (loginDTO == null)
                return new BaseResponseDTO<String>
                {
                    Flag = false,
                    Message = "Model is empty",
                };

            //Get user by email
            var getUser = await userManager.FindByEmailAsync(loginDTO.Email);

            //Check user is not null
            if (getUser is null)
                return new BaseResponseDTO<String>
                {
                    Flag = false,
                    Message = "User not found"
                };

            //Check user password is correct
            bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPasswords)
                return new BaseResponseDTO<String>
                {
                    Flag = false,
                    Message = "Incorrect password/username"
                };

            //generate jwt token
            var getUserRole = await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession()
            {
                Id = getUser.Id,
                Email = getUser.Email,
                Role = getUserRole.First()
            };

            string token = GenerateToken(userSession);
            return new BaseResponseDTO<string>
            {
                Flag = true,
                Message = "Login successful",
                Data = token
            };
        }
        public async Task<BaseResponseDTO> UpdateAccount(ApplicationUser userDTO)
        {
            //Check model is empty
            if (userDTO is null) return new BaseResponseDTO
            {
                Flag = false,
                Message = "Model is empty"
            };

            //Get user by email
            var user = await userManager.FindByEmailAsync(userDTO.Email);
            //Check user is not null
            if (user is null)
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "User not found"
                };

            //Assign non empty updated values into user model
            if (userDTO.FirstName != null) user.FirstName = userDTO.FirstName;
            if (userDTO.LastName != null) user.LastName = userDTO.LastName;
            if (userDTO.Street != null) user.Street = userDTO.Street;
            if (userDTO.City != null) user.City = userDTO.City;
            if (userDTO.PostalCode != null) user.PostalCode = userDTO.PostalCode;
            if (userDTO.Province != null) user.Province = userDTO.Province;
            if (userDTO.Longitude != 0) user.Longitude = userDTO.Longitude;
            if (userDTO.Latitude != 0) user.Latitude = userDTO.Latitude;

            //Store updated image
            if (userDTO.ProfilePicture != null)
            {
                var (condition, filepath) = await cloudProvider.UploadFile(userDTO.ProfilePicture, config["StorageDirectories:ProfileImages"]);
                if (condition)
                {
                    //remove previous image from storage
                    if (user.ProfilePicturePath != null)
                    {
                        await cloudProvider.RemoveFile(user.ProfilePicturePath);
                    }
                    //assign new image
                    user.ProfilePicturePath = filepath;
                }
                else
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = filepath
                    };
                }
            }
            {
                var (condition, filepath) = await cloudProvider.UploadFile(userDTO.ProfilePicture, config["StorageDirectories:ProfileImages"]);
                if (condition)
                {
                    //remove previous image from storage
                    if (user.ProfilePicturePath != null)
                    {
                        await cloudProvider.RemoveFile(user.ProfilePicturePath);
                    }
                    //assign new image
                    user.ProfilePicturePath = filepath;
                }
                else
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = filepath
                    };
                }
            }


            //Update user
            var updatedUser = await userManager.UpdateAsync(user);
            if (!updatedUser.Succeeded)
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = updatedUser.ToString()
                };

            //return response
            return new BaseResponseDTO
            {
                Flag = true,
                Message = "Account updated"
            };
        }
        //Get currrent logged in user details
        public async Task<BaseResponseDTO<UserResponseDTO>> GetMe()
        {
            var result = string.Empty;
            if (httpContextAccessor.HttpContext != null)
            {
                result = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                Console.WriteLine($"Id : {result}");
                if (result != null)
                {
                    //Get user
                    var currentUser = await userManager.FindByEmailAsync(result);

                    //Check user if exist
                    if (currentUser != null)
                    {
                        UserResponseDTO userResponse = new UserResponseDTO
                        {
                            Id = currentUser.Id,
                            FirstName = currentUser.FirstName,
                            LastName = currentUser.LastName,
                            Street = currentUser.Street,
                            City = currentUser.City,
                            PostalCode = currentUser.PostalCode,
                            Province = currentUser.Province,
                            Longitude = currentUser.Longitude,
                            Latitude = currentUser.Latitude,
                            Email = currentUser.Email,
                        };
                        //Get user Profile Picture URL
                        if (currentUser.ProfilePicturePath != null)
                        {
                            userResponse.ProfilePictureURL = cloudProvider.GeneratePreSignedUrlForDownload(currentUser.ProfilePicturePath);
                        }
                        return new BaseResponseDTO<UserResponseDTO>
                        {
                            Flag = true,
                            Message = "User found",
                            Data = userResponse

                        };
                    }
                }
            }
            return new BaseResponseDTO<UserResponseDTO>
            {
                Flag = false,
                Message = "User not found",
            };
        }
        //Generate JWT token
        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<BaseResponseDTO> DeleteAccount(string userId)
        {
            //Check user id is empty
            if (userId == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "User not found"
                };
            }

            //Get user by id
            var user = await userManager.FindByIdAsync(userId);

            //Check user is not null
            if (user is null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "User not found"
                };
            }

            // Check if user has any shops
            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var shop = await context.Shops.FirstOrDefaultAsync(x => x.OwnerId == userId);
                if (shop != null)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "User has shops"
                    };
                }
            }

            // Check user has ordered any services
            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var package = await context.Packages.FirstOrDefaultAsync(x => x.UserId == userId);
                if (package != null)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "User has ordered services"
                    };
                }
            }

            // Remove user profile picture from storage
            if (user.ProfilePicturePath != null && user.ProfilePicturePath != "images/profile-images/default.png")
            {
                await cloudProvider.RemoveFile(user.ProfilePicturePath);
            }

            // Delete user
            var deletedUser = await userManager.DeleteAsync(user);
            if (!deletedUser.Succeeded)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = deletedUser.ToString()
                };
            }

            return new BaseResponseDTO
            {
                Flag = true,
                Message = "Account deleted"
            };


        }
        public async Task<BaseResponseDTO> UpdatePassword(string userId, string oldPassword, string newPassword)
        {
            //Check user id is empty
            if (userId == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "User not found"
                };
            }

            // Check old password  and new password is empty or same
            if (oldPassword == null || newPassword == null || oldPassword == newPassword)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "Invalid password"
                };
            }

            //Get user by id
            var user = userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "User not found"
                };
            }

            //Check old password is correct
            var checkPassword = await userManager.CheckPasswordAsync(user, oldPassword);
            if (!checkPassword)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "Incorrect password"
                };
            }

            //Change password
            var changePassword = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
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
    }
}
