using System.Security.Claims;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class ShopServiceRepository(UserManager<ApplicationUser> userManager, IUserAccountRepository userAccountRepository,
                                            IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor) : IShopServiceRepository
    {
        public async Task<ShopResponse> CreateShop(ShopRequestDTO shopRequestDTO)
        {
            if (shopRequestDTO == null)
            {
                return new ShopResponse(false, "shopReqeustDTO is null", null);
            }

            if (httpContextAccessor.HttpContext?.User != null)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var result = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                        if (result == null)
                        {
                            return new ShopResponse(false, "owner cannot found", null);
                        }

                        var currentUser = await userManager.FindByEmailAsync(result);
                        if (currentUser == null)
                        {
                            return new ShopResponse(false, "owner cannot found", null);
                        }

                        ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                        if (dbContext == null)
                        {
                            return new ShopResponse(false, "Internal server error!", null);
                        }

                        //check if shop already exists
                        var shop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstOrDefaultAsync();
                        if (shop != null)
                        {
                            return new ShopResponse(false, "Shop already exists", null);
                        }

                        var newShop = new Shop()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = shopRequestDTO.Name,
                            Description = shopRequestDTO.Description,
                            Rating = shopRequestDTO.Rating,
                            OwnerId = currentUser.Id
                        };

                        //add the new shop to the database
                        await dbContext.Shops.AddAsync(newShop);
                        await dbContext.SaveChangesAsync();

                        var createdShop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstOrDefaultAsync();
                        if (createdShop != null)
                        {
                            var createdShopResponseDTO = new ShopResponseDTO()
                            {
                                Id = createdShop.Id,
                                Name = createdShop.Name,
                                Description = createdShop.Description,
                                Rating = createdShop.Rating,
                            };

                            //add the vendor role to the current user
                            await userManager.RemoveFromRoleAsync(currentUser, "client");
                            await userManager.AddToRoleAsync(currentUser, "vendor");
                            return new ShopResponse(true, "Shop created successfully", createdShopResponseDTO);
                        }
                        else
                        {
                            return new ShopResponse(false, "Shop cannot found", null);
                        }

                    }
                    catch (Exception ex)
                    {
                        return new ShopResponse(false, $"Internal server error! {ex.Message}", null);
                    }
                }
            }
            else
            {
                return new ShopResponse(false, "owner cannot found", null);
            }

        }
        public async Task<GeneralResponse> DeleteShop()
        {
            //check weather user is the owner of the shop
            var currentUserResponse = await userAccountRepository.GetMe();
            if (currentUserResponse.Flag == false)
            {
                return new GeneralResponse(false, "owner cannot found");
            }

            //get the current user
            var currentUser = await userManager.FindByIdAsync(currentUserResponse.userResponseDTO?.Id ?? string.Empty);
            if (currentUser == null)
            {
                return new GeneralResponse(false, "owner cannot found");
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (dbContext == null)
                {
                    return new GeneralResponse(false, "Internal server error!");
                }
                try
                {
                    var shop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstAsync();
                    if (shop == null)
                    {
                        return new GeneralResponse(false, "Shop not found!");
                    }
                    dbContext.Shops.Remove(shop);
                    await dbContext.SaveChangesAsync();
                    return new GeneralResponse(true, "Shop deleted successfully");
                }
                catch (Exception ex)
                {
                    return new GeneralResponse(false, $"Internal server error! {ex}");
                }
            }
        }
        public async Task<ShopResponse> GetMyShop()
        {
            //get the current login user(owner of the shop)
            var currentUserResponse = await userAccountRepository.GetMe();
            if (currentUserResponse.Flag == false)
            {
                return new ShopResponse(false, "owner cannot found", null);
            }
            try
            {
                if (currentUserResponse.userResponseDTO.Id == null)
                {
                    return new ShopResponse(false, "owner cannot found", null);
                }
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    if (dbContext == null)
                    {
                        return new ShopResponse(false, "Internal server error!", null);
                    }
                    ApplicationUser? currentUser = await userManager.FindByIdAsync(currentUserResponse.userResponseDTO.Id);
                    if (currentUser == null)
                    {
                        return new ShopResponse(false, "owner cannot found", null);
                    }
                    var shop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstAsync();
                    if (shop == null)
                    {
                        return new ShopResponse(false, "Shop not found!", null);
                    }
                    var shopResponseDTO = new ShopResponseDTO()
                    {
                        Id = shop.Id,
                        Description = shop.Description,
                        Name = shop.Name,
                        Rating = shop.Rating,
                    };
                    return new ShopResponse(true, "Shop found successfully", shopResponseDTO);
                }
            }
            catch (Exception ex)
            {
                return new ShopResponse(false, $"Internal server error! {ex}", null);
            }
        }
        public async Task<ShopResponse> UpdateShop(ShopRequestDTO shopRequestDTO)
        {
            //get the current login user(owner of the shop)
            var currentUserResponse = await userAccountRepository.GetMe();
            if (currentUserResponse.Flag == false)
            {
                return new ShopResponse(false, "owner cannot found", null);
            }
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                    if (dbContext == null)
                    {
                        return new ShopResponse(false, "Internal server error!", null);
                    }

                    var currentUser = await userManager.FindByIdAsync(currentUserResponse.userResponseDTO.Id ?? string.Empty);
                    if (currentUser == null)
                    {
                        return new ShopResponse(false, "owner cannot found", null);
                    }
                    var shop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstAsync();
                    if (shop == null)
                    {
                        return new ShopResponse(false, "Shop not found!", null);
                    }
                    shop.Name = shopRequestDTO.Name;
                    shop.Description = shopRequestDTO.Description;
                    shop.Rating = shopRequestDTO.Rating;
                    dbContext.Shops.Update(shop);
                    await dbContext.SaveChangesAsync();

                    var shopResponseDTO = new ShopResponseDTO()
                    {
                        Id = shop.Id,
                        Name = shop.Name,
                        Description = shop.Description,
                        Rating = shop.Rating,
                    };
                    return new ShopResponse(true, "Shop updated successfully", shopResponseDTO);
                }
            }
            catch (Exception ex)
            {
                return new ShopResponse(false, $"Internal server error! {ex}", null);
            }
        }
        public async Task<ShopListResponse> GetAllShops()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (dbContext == null)
                {
                    return new ShopListResponse(false, "Internal server error!", null);
                }
                try
                {
                    var shops = await dbContext.Shops.ToListAsync();
                    if (shops.Count == 0)
                    {
                        return new ShopListResponse(false, "No shop found!", null);
                    }
                    List<ShopResponseDTO> shopResponseDTOs = new List<ShopResponseDTO>();
                    foreach (var shop in shops)
                    {
                        var shopResponseDTO = new ShopResponseDTO()
                        {
                            Id = shop.Id,
                            Name = shop.Name,
                            Description = shop.Description,
                            Rating = shop.Rating,
                        };
                        shopResponseDTOs.Add(shopResponseDTO);
                    }
                    return new ShopListResponse(true, "Shops found successfully", shopResponseDTOs);
                }
                catch (Exception ex)
                {
                    return new ShopListResponse(false, $"Internal server error! {ex}", null);
                }
            }
        }
        
    }
}
