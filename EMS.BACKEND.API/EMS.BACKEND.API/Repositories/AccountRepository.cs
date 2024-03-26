using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
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
        IConfiguration config, IHttpContextAccessor httpContextAccessor, IFileService fileService, ICloudProviderRepository cloudProvider) : IUserAccountRepository
    {
        public async Task<LoginResponse> CreateAccount(UserRequestDTO userDTO)
        {
            if (userDTO is null) return new LoginResponse(false, null!, "Model is empty");

            var newUser = new ApplicationUser()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                PasswordHash = userDTO.Password,
                UserName = userDTO.Email,
                Street = userDTO.Street,
                City = userDTO.City,
                PostalCode = userDTO.PostalCode,
                Province = userDTO.Province,
                Longitude = userDTO.Longitude,
                Latitude = userDTO.Latitude,
            };

            //store image
            var (condition, filepath) = await fileService.UploadFile(userDTO.ProfilePicture, "Images");
            if (condition)
            {
                newUser.ProfilePicture = filepath;
            }
            else
            {
                return new LoginResponse(false, null!, filepath);
            }
            var user = await userManager.FindByEmailAsync(newUser.Email);
            if (user is not null)
                return new LoginResponse(false, null!, "User registered already");

            var createUser = await userManager.CreateAsync(newUser, userDTO.Password);
            if (!createUser.Succeeded)
                return new LoginResponse(false, null!, createUser.ToString());

            //Assign Default Role : "client"
            await userManager.AddToRoleAsync(newUser, "client");


            //generate jwt token
            var getUser = await userManager.FindByEmailAsync(newUser.Email);
            if (getUser is not null)
            {
                var getUserRole = await userManager.GetRolesAsync(getUser);
                var userSession = new UserSession(getUser.Id, getUser.Email, getUserRole.First());
                string token = GenerateToken(userSession);
                return new LoginResponse(true, token!, "Login completed");
            }

            return new LoginResponse(true, null!, "Account Created");
        }
        public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                return new LoginResponse(false, null!, "Login container is empty");

            var getUser = await userManager.FindByEmailAsync(loginDTO.Email);
            if (getUser is null)
                return new LoginResponse(false, null!, "User not found");

            bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPasswords)
                return new LoginResponse(false, null!, "Invalid email/password");

            var getUserRole = await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.Email, getUserRole.First());
            string token = GenerateToken(userSession);
            return new LoginResponse(true, token!, "Login completed");
        }
        //Update Account Details
        public async Task<GeneralResponse> UpdateAccount(UserRequestDTO userDTO)
        {
            if (userDTO is null) return new GeneralResponse(false, "Model is empty");
            //Get user
            var user = await userManager.FindByEmailAsync(userDTO.Email);
            if (user is null)
                return new GeneralResponse(false, "User account is not registered ");

            //Assing new values
            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;
            user.Street = userDTO.Street;
            user.City = userDTO.City;
            user.PostalCode = userDTO.PostalCode;
            user.Province = userDTO.Province;
            user.Longitude = userDTO.Longitude;
            user.Latitude = userDTO.Latitude;

            //Store updated image
            if (userDTO.ProfilePicture != null)
            {
                var (condition, filepath) = await fileService.UploadFile(userDTO.ProfilePicture, "Images");
                if (condition)
                {
                    user.ProfilePicture = filepath;
                }
            }


            //Update user
            var updatedUser = await userManager.UpdateAsync(user);
            if (!updatedUser.Succeeded)
                return new GeneralResponse(false, updatedUser.ToString());

            //return response
            return new GeneralResponse(true, "Account Updated");
        }
        //Reset Password
        //public async Task<GeneralResponse> ResetUserPassowrd(UserRequestDTO userDTO);
        //Get currrent logged in user details
        public async Task<UserResponse> GetMe()
        {
            var result = string.Empty;
            if (httpContextAccessor.HttpContext != null)
            {
                result = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                Console.WriteLine($"Id : {result}");
                if (result != null)
                {
                    var currentUser = await userManager.FindByEmailAsync(result);
                    if (currentUser != null)
                    {
                        var (condition, bitStream, contentType, message) = await fileService.DownloadFiles(currentUser.ProfilePicture);
                        if (condition)
                        {
                            var currentUserResponse = new UserResponseDTO()
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
                            return new UserResponse(true, "successfully found", currentUserResponse);
                        }
                        else
                        {
                            return new UserResponse(false, message, null);
                        }
                    }
                }
            }
            return new UserResponse(false, "User cannot found!", null);
        }

        public string GeneratePreSignedUrl()
        {
            return cloudProvider.GeneratePreSignedUrlForDownload();
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
