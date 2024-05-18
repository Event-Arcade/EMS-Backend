using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.Shop;
using EMS.BACKEND.API.Mappers;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class ShopRepository : IShopRepository
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICloudProviderRepository _cloudProvider;
        private readonly ITokenService _tokenService;

        public ShopRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration,
                                IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, ITokenService tokenService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _cloudProvider = cloudProvider;
            _tokenService = tokenService;
        }

        public async Task<BaseResponseDTO<string,ShopResponseDTO>> CreateAsync(string userId, ShopCreateDTO entity)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    // get the user and check if the user is a vendor already
                    var user = await _userManager.FindByIdAsync(userId);
                    var userRole = await _userManager.GetRolesAsync(user);
                    if (userRole.Contains("vendor"))
                    {
                        return new BaseResponseDTO<string, ShopResponseDTO>
                        {
                            Message = "User is already a vendor",
                            Flag = false
                        };
                    }

                    // Assign the shop id and owner id
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


                    //upload the background image
                    var (flag, path) = await _cloudProvider
                    .UploadFile(entity.BackGroundImage, _configuration["StorageDirectories:ShopImages"]);
                    if (!flag)
                    {
                        throw new Exception("Failed to upload the file to the cloud");
                    }

                    // Create the shop
                    var shop = entity.MapToShop(user, path);

                    await dbContext.Shops.AddAsync(shop);
                    await dbContext.SaveChangesAsync();

                    // upgrade user role "client" to "vendor"
                    await _userManager.AddToRoleAsync(user, "vendor");
                    await _userManager.RemoveFromRoleAsync(user, "client");

                    // Generate a new token for the user
                    var getUserRole = await _userManager.GetRolesAsync(user);
                    var token = _tokenService.CreateToken(user, getUserRole.First());

                    // get the newly created shop
                    var newShop = dbContext.Shops.Where(s => s.OwnerId == user.Id).Include(s => s.Services).FirstOrDefault();

                    // convert the background image path to a url
                    var url = _cloudProvider.GeneratePreSignedUrlForDownload(newShop.BackgroundImagePath);

                    return new BaseResponseDTO<string, ShopResponseDTO>
                    {
                        Message = "Shop created successfully",
                        Flag = true,
                        Data2 = newShop.MapToShopResponseDTO(url),
                        Data1 = token
                    };

                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<string, ShopResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO> DeleteAsync(string userId, int id)

        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // get current user and check if the user is a vendor
                    var user = await _userManager.FindByIdAsync(userId);
                    var userRole = await _userManager.GetRolesAsync(user);
                    if (!userRole.Contains("vendor"))
                    {
                        return new BaseResponseDTO
                        {
                            Message = "User is not a vendor",
                            Flag = false
                        };
                    }

                    var shop = await dbContext.Shops.Where(s => s.Id == id).FirstOrDefaultAsync();

                    if (shop == null)
                    {
                        return new BaseResponseDTO { Message = "Shop not found", Flag = false };
                    }

                    //check if the user is the owner of the shop
                    if (shop.OwnerId != user.Id)
                    {
                        return new BaseResponseDTO { Message = "You are not the owner of the shop", Flag = false };
                    }

                    // Remove the background image from the cloud
                    if (shop.BackgroundImagePath != "images/shop-images/default.png")
                    {
                        var flag = await _cloudProvider.RemoveFile(shop.BackgroundImagePath);
                        if (!flag)
                        {
                            throw new Exception("Failed to delete the file from the cloud");
                        }
                    }

                    //delete the shop
                    dbContext.Shops.Remove(shop);
                    await dbContext.SaveChangesAsync();

                    // assign the user role "vendor" to "client"
                    await _userManager.AddToRoleAsync(user, "client");
                    await _userManager.RemoveFromRoleAsync(user, "vendor");

                    // Generate a new token for the user
                    var getUserRole = await _userManager.GetRolesAsync(user);
                    var token = _tokenService.CreateToken(user, getUserRole.First());

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
                return new BaseResponseDTO { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<IEnumerable<ShopResponseDTO>>> FindAllAsync()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shops = await dbContext.Shops.ToListAsync();

                    //convert the background image path to a url
                    foreach (var shop in shops)
                    {
                        var url = _cloudProvider.GeneratePreSignedUrlForDownload(shop.BackgroundImagePath);
                        shop.BackgroundImagePath = url;
                    }

                    var shopDTOs = shops.Select(s => s.MapToShopResponseDTO(s.BackgroundImagePath));

                    return new BaseResponseDTO<IEnumerable<ShopResponseDTO>>
                    {
                        Data = shopDTOs,
                        Flag = true,
                        Message = "Shops found"
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<ShopResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<ShopResponseDTO>> FindByIdAsync(int id)
        {
            try
            {

                //get the shop
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shop = await dbContext.Shops.Where(s => s.Id == id).FirstOrDefaultAsync();

                    if (shop == null)
                    {
                        return new BaseResponseDTO<ShopResponseDTO> { Message = "Shop not found", Flag = false };
                    }

                    //convert the background image path to a url
                    var shopURL = _cloudProvider.GeneratePreSignedUrlForDownload(shop.BackgroundImagePath);

                    return new BaseResponseDTO<ShopResponseDTO>
                    {
                        Data = shop.MapToShopResponseDTO(shopURL),
                        Flag = true,
                        Message = "Shop found"
                    };
                }

            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ShopResponseDTO> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<ShopResponseDTO>> GetShopByVendor(string userId)
        {
            try
            {
                // get the user and check if the user is a vendor already
                var user = await _userManager.FindByIdAsync(userId);
                var userRole = await _userManager.GetRolesAsync(user);
                if (!userRole.Contains("vendor"))
                {
                    return new BaseResponseDTO<ShopResponseDTO>
                    {
                        Message = "User is not a vendor",
                        Flag = false
                    };
                }

                //get the shop
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shop = await dbContext.Shops.Where(s => s.OwnerId == user.Id).FirstOrDefaultAsync();

                    if (shop == null)
                    {
                        return new BaseResponseDTO<ShopResponseDTO>
                        {
                            Message = "Service not found",
                            Flag = false
                        };
                    }

                    //convert the background image path to a url
                    var url = _cloudProvider.GeneratePreSignedUrlForDownload(shop.BackgroundImagePath);

                    return new BaseResponseDTO<ShopResponseDTO>
                    {
                        Data = shop.MapToShopResponseDTO(url),
                        Flag = true,
                        Message = "Welcome to ${shop.Name}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ShopResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<ShopResponseDTO>> UpdateAsync(string userId, int id, ShopCreateDTO entity)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shop = await dbContext.Shops.Where(s => s.Id == id).FirstOrDefaultAsync();

                    if (shop == null)
                    {
                        return new BaseResponseDTO<ShopResponseDTO> { Message = "Shop not found", Flag = false };
                    }

                    //get the user
                    var user = await _userManager.FindByIdAsync(userId);

                    //check if the user is the owner of the shop
                    if (shop.OwnerId != user.Id)
                    {
                        return new BaseResponseDTO<ShopResponseDTO>
                        {
                            Message = "You are not the owner of the shop",
                            Flag = false
                        };
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
                    if (entity.BackGroundImage != null)
                    {
                        var (flag, path) = await _cloudProvider
                                .UpdateFile(entity.BackGroundImage, _configuration["StorageDirectories:ShopImages"], shop.BackgroundImagePath);
                        if (!flag)
                        {
                            throw new Exception("Failed to upload the file to the cloud");
                        }
                        shop.BackgroundImagePath = path;
                    }

                    dbContext.Shops.Update(shop);
                    await dbContext.SaveChangesAsync();
                    string backgroundImageUrl = _cloudProvider.GeneratePreSignedUrlForDownload(shop.BackgroundImagePath);
                    return new BaseResponseDTO<ShopResponseDTO>
                    {
                        Message = "Shop updated successfully",
                        Flag = true,
                        Data = shop.MapToShopResponseDTO(backgroundImageUrl)
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ShopResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false,
                };
            }
        }
    }
}
