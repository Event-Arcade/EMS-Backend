using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharedClassLibrary.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EMS.BACKEND.API.Repositories
{
    public class AccountRepository(UserManager<ApplicationUser> userManager,
        IConfiguration config, IHttpContextAccessor httpContextAccessor, ICloudProviderRepository cloudProvider) : IUserAccountRepository
    {
        public async Task<BaseResponseDTO> CreateAccount(UserRequestDTO userDTO)
        {
            //Check model is empty
            if (userDTO is null) return new BaseResponseDTO
            {
                Flag = false,
                Message = "Model is empty"
            };

            //check weather user already registered
            var user = await userManager.FindByEmailAsync(userDTO.Email);
            if (user is not null)
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "User already registered"
                };

            //create new user object
            var newUser = new ApplicationUser()
            {
                Email = userDTO.Email,
                UserName = userDTO.FirstName,
                Street = userDTO.Street,
                City = userDTO.City,
                PostalCode = userDTO.PostalCode,
                Province = userDTO.Province,
                Longitude = userDTO.Longitude,
                Latitude = userDTO.Latitude,
            };

            //store profile-picture in storage
            var (condition, filepath) = await cloudProvider.UploadFile(userDTO.ProfilePicture, config["StorageDirectories:ProfileImages"]);
            if (condition)
            {
                newUser.ProfilePicture = filepath;
            }
            else
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = filepath
                };
            }

            var createUser = await userManager.CreateAsync(newUser, userDTO.Password);
            //Check user created
            if (!createUser.Succeeded)
                return new BaseResponseDTO
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

            return new BaseResponseDTO
            {
                Flag = false,
                Message = "Error occured while creating account"
            };
        }
        public async Task<BaseResponseDTO> LoginAccount(LoginDTO loginDTO)
        {
            //Check login container is empty
            if (loginDTO == null)
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "Model is empty"
                };

            //Get user by email
            var getUser = await userManager.FindByEmailAsync(loginDTO.Email);

            //Check user is not null
            if (getUser is null)
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "User not found"
                };

            //Check user password is correct
            bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPasswords)
                return new BaseResponseDTO
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
        //Update Account Details
        public async Task<BaseResponseDTO> UpdateAccount(UserRequestDTO userDTO)
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

            //Assing new values
            user.UserName = userDTO.FirstName;
            user.Street = userDTO.Street;
            user.City = userDTO.City;
            user.PostalCode = userDTO.PostalCode;
            user.Province = userDTO.Province;
            user.Longitude = userDTO.Longitude;
            user.Latitude = userDTO.Latitude;

            //Store updated image
            if (userDTO.ProfilePicture != null)
            {
                var (condition, filepath) = await cloudProvider.UploadFile(userDTO.ProfilePicture, config["StorageDirectories:ProfileImages"]);
                if (condition)
                {
                    //remove previous image from storage
                    if (user.ProfilePicture != null)
                    {
                        await cloudProvider.RemoveFile(user.ProfilePicture);
                    }
                    //assign new image
                    user.ProfilePicture = filepath;
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
        //Reset Password
        //public async Task<GeneralResponse> ResetUserPassowrd(UserRequestDTO userDTO);
        //Get currrent logged in user details
        public async Task<BaseResponseDTO<ApplicationUser>> GetMe()
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
                        //Get user Profile Picture URL
                        if (currentUser.ProfilePicture != null)
                        {
                            currentUser.ProfilePicture = cloudProvider.GeneratePreSignedUrlForDownload(currentUser.ProfilePicture);
                        }
                        return new BaseResponseDTO<ApplicationUser>
                        {
                            Flag = true,
                            Message = "User found",
                            Data = currentUser
                        };
                    }
                }
            }
            return new BaseResponseDTO<ApplicationUser>
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
    }
}
