using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class ShopServiceRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration, IUserAccountRepository userAccountRepository,
                                        IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor,
                                            ICloudProviderRepository cloudProvider, IServiceRepository serviceRepository) : IShopServiceRepository
    {
        public async Task<BaseResponseDTO<String>> CreateAsync(Shop entity)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    //check if the entity is null
                    if (entity == null)
                    {
                        throw new ArgumentNullException(nameof(entity));
                    }
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    //get the user
                    var user = await userManager.FindByEmailAsync(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value);

                    // Check Weather the user is a vendor already
                    var userRole = await userManager.GetRolesAsync(user);
                    if (userRole.Contains("vendor"))
                    {
                        return new BaseResponseDTO<String>
                        {
                            Message = "User is already a vendor",
                            Flag = false
                        };
                    }

                    // Assign the shop id and owner id
                    entity.Id = Guid.NewGuid().ToString();
                    entity.OwnerId = user.Id;

                    //upload the background image
                    var (flag, path) = await cloudProvider
                    .UploadFile(entity.BackgroundImage, configuration["StorageDirectories:ShopImages"]);
                    if (!flag)
                    {
                        throw new Exception("Failed to upload the file to the cloud");
                    }
                    entity.BackgroundImagePath = path;


                    await dbContext.Shops.AddAsync(entity);
                    await dbContext.SaveChangesAsync();

                    // upgrade user role "client" to "vendor"
                    await userManager.AddToRoleAsync(user, "vendor");
                    await userManager.RemoveFromRoleAsync(user, "client");

                    // Generate a new token for the user
                    var getUserRole = await userManager.GetRolesAsync(user);
                    var userSession = new UserSession()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Role = getUserRole.First()
                    };
                    var token = GenerateToken(userSession);
                    return new BaseResponseDTO<String>
                    {
                        Message = "Shop created successfully",
                        Flag = true,
                        Data = token
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<String>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<String>> DeleteAsync(string id)
        {
            try
            {
                //check if the id is null
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // get current user
                    var shopOwner = await userAccountRepository.GetMe();

                    var shop = await dbContext.Shops.FindAsync(id);

                    if (shop == null)
                    {
                        return new BaseResponseDTO<String> { Message = "Shop not found", Flag = false };
                    }

                    //check if the user is the owner of the shop
                    if (shop.OwnerId != shopOwner.Data.Id)
                    {
                        return new BaseResponseDTO<String> { Message = "You are not the owner of the shop", Flag = false };
                    }
                    //check if there any services associated with the shop
                    var services = await dbContext.Services.Where(s => s.ShopId == id).ToListAsync();
                    if (services.Count > 0)
                    {
                        return new BaseResponseDTO<String> { Message = "Shop has services associated with it", Flag = false };
                    }

                    // Remove the background image from the cloud
                    var flag = await cloudProvider.RemoveFile(shop.BackgroundImagePath);
                    if (!flag)
                    {
                        throw new Exception("Failed to delete the file from the cloud");
                    }

                    //delete the shop
                    dbContext.Shops.Remove(shop);
                    dbContext.SaveChanges();

                    // assign the user role "vendor" to "client"
                    var user = await userManager.FindByIdAsync(shop.OwnerId);
                    await userManager.AddToRoleAsync(user, "client");
                    await userManager.RemoveFromRoleAsync(user, "vendor");

                    // Generate a new token for the user
                    var getUserRole = await userManager.GetRolesAsync(user);
                    var userSession = new UserSession()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Role = getUserRole.First()
                    };
                    var token = GenerateToken(userSession);

                    return new BaseResponseDTO<String>
                    {
                        Message = "Shop deleted successfully",
                        Flag = true,
                        Data = token
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<String> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<IEnumerable<Shop>>> FindAllAsync()
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shops = await dbContext.Shops.ToListAsync();

                    //convert the background image path to a url
                    foreach (var shop in shops)
                    {
                        var url = cloudProvider.GeneratePreSignedUrlForDownload(shop.BackgroundImagePath);
                        shop.BackgroundImagePath = url;
                    }

                    //get services associated with the shop
                    foreach (var s in shops)
                    {
                        var servicesResponse = await serviceRepository.GetServicesByShopId(s.Id);
                        if (servicesResponse.Flag)
                        {
                            List<Service> services = new List<Service>();
                            foreach (var service in servicesResponse.Data)
                            {
                                services.Add(service);
                            }
                            s.Services = services;
                        }
                    }

                    return new BaseResponseDTO<IEnumerable<Shop>> { Data = shops, Flag = true, Message = "Shops found" };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<Shop>> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<Shop>> FindByIdAsync(string id)
        {
            try
            {
                //check if the id is null
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                //get the shop
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shop = await dbContext.Shops.Where(s => s.Id == id).FirstOrDefaultAsync();

                    if (shop == null)
                    {
                        return new BaseResponseDTO<Shop> { Message = "Shop not found", Flag = false };
                    }

                    //convert the background image path to a url
                    var url = cloudProvider.GeneratePreSignedUrlForDownload(shop.BackgroundImagePath);
                    shop.BackgroundImagePath = url;

                    //get services associated with the shop
                    var servicesResponse = await serviceRepository.GetServicesByShopId(shop.Id);
                    if (servicesResponse.Flag)
                    {
                        List<Service> services = new List<Service>();
                        foreach (var service in servicesResponse.Data)
                        {
                            services.Add(service);
                        }
                        shop.Services = services;
                    }

                    return new BaseResponseDTO<Shop> { Data = shop, Flag = true, Message = "Shop found" };
                }

            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<Shop> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<Shop>> GetShopByServiceId(string serviceId)
        {
            try
            {
                //check if the serviceId is null
                if (serviceId == null)
                {
                    throw new ArgumentNullException(nameof(serviceId));
                }

                //get the shop
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var service = await dbContext.Services.Where(s => s.Id == serviceId).FirstOrDefaultAsync();

                    if (service == null)
                    {
                        return new BaseResponseDTO<Shop> { Message = "Service not found", Flag = false };
                    }

                    //get the shop
                    var response = await FindByIdAsync(service.ShopId);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<Shop> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<Shop>> GetShopByVendor()
        {
            //TODO: services are not add with response
            try
            {
                //get the user 
                var result = await userAccountRepository.GetMe();
                if (!result.Flag)
                {
                    return new BaseResponseDTO<Shop> { Message = "User not found", Flag = false };
                }

                var user = result.Data;

                //get the shop
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shop = await dbContext.Shops.Where(s => s.OwnerId == user.Id).FirstOrDefaultAsync();

                    if (shop == null)
                    {
                        return new BaseResponseDTO<Shop> { Message = "Service not found", Flag = false };
                    }

                    //convert the background image path to a url
                    var url = cloudProvider.GeneratePreSignedUrlForDownload(shop.BackgroundImagePath);
                    shop.BackgroundImagePath = url;

                    //get the service associated with the shop
                    var servicesResponse = await serviceRepository.GetServicesByShopId(shop.Id);
                    if (servicesResponse.Flag)
                    {
                        foreach (var service in servicesResponse.Data)
                        {
                            shop.Services.Add(service);
                        }
                    }

                    return new BaseResponseDTO<Shop> { Data = shop, Flag = true, Message = "Shop found" };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<Shop> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO> UpdateAsync(String id, Shop entity)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    //check if the entity is null
                    if (entity == null)
                    {
                        throw new ArgumentNullException(nameof(entity));
                    }
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shop = await dbContext.Shops.Where(s => s.Id == id).FirstOrDefaultAsync();

                    if (shop == null)
                    {
                        return new BaseResponseDTO { Message = "Shop not found", Flag = false };
                    }

                    //get the user
                    var user = await userManager.FindByEmailAsync(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value);

                    //check if the user is the owner of the shop
                    if (shop.OwnerId != user.Id)
                    {
                        return new BaseResponseDTO { Message = "You are not the owner of the shop", Flag = false };
                    }

                    //update the shop
                    if (entity.Name != null)
                    {
                        shop.Name = entity.Name;
                    }
                    if (entity.Description != null)
                    {
                        shop.Description = entity.Description;
                    }
                    //TODO: calculate the rating automatically

                    //upload the background image
                    if (entity.BackgroundImage != null)
                    {
                        var (flag, path) = await cloudProvider
                                .UpdateFile(entity.BackgroundImage, configuration["StorageDirectories:ShopImages"], shop.BackgroundImagePath);
                        if (!flag)
                        {
                            throw new Exception("Failed to upload the file to the cloud");
                        }
                        shop.BackgroundImagePath = path;
                    }


                    dbContext.Shops.Update(shop);
                    await dbContext.SaveChangesAsync();
                    return new BaseResponseDTO { Message = "Shop updated successfully", Flag = true };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Message = ex.Message, Flag = false };
            }
        }
        //Generate JWT token
        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
