using System.Security.Claims;
using EMS.BACKEND.API.Contracts;
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
                                            IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor, ICloudProviderRepository cloudProvider) : IShopServiceRepository
    {
        // public async Task<BaseResponseDTO> CreateShop(ShopRequestDTO shopRequestDTO)
        // {
        //     //check if shopRequestDTO is null
        //     if (shopRequestDTO == null)
        //     {
        //         return new BaseResponseDTO
        //         {
        //             Flag = false,
        //             Message = "Shop request cannot be null"
        //         };
        //     }

        //     //check if shopRequestDTO.Name is null
        //     if (httpContextAccessor.HttpContext?.User != null)
        //     {
        //         using (var scope = serviceScopeFactory.CreateScope())
        //         {
        //             try
        //             {
        //                 //get the current login user(owner of the shop)
        //                 var result = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        //                 //check if the owner is null
        //                 if (result == null)
        //                 {
        //                     return new BaseResponseDTO
        //                     {
        //                         Flag = false,
        //                         Message = "owner cannot found"
        //                     };
        //                 }

        //                 //get the current user
        //                 var currentUser = await userManager.FindByEmailAsync(result);
        //                 if (currentUser == null)
        //                 {
        //                     return new BaseResponseDTO
        //                     {
        //                         Flag = false,
        //                         Message = "owner cannot found"
        //                     };
        //                 }

        //                 ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

        //                 //check if shop already exists
        //                 var shop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstOrDefaultAsync();
        //                 if (shop != null)
        //                 {
        //                     return new BaseResponseDTO
        //                     {
        //                         Flag = false,
        //                         Message = "Shop already exists"
        //                     };
        //                 }

        //                 //create a new shop object
        //                 var newShop = new Shop()
        //                 {
        //                     Id = Guid.NewGuid().ToString(),
        //                     Name = shopRequestDTO.Name,
        //                     Description = shopRequestDTO.Description,
        //                     Rating = shopRequestDTO.Rating,
        //                     OwnerId = currentUser.Id
        //                 };

        //                 //add the new shop to the database
        //                 await dbContext.Shops.AddAsync(newShop);
        //                 await dbContext.SaveChangesAsync();

        //                 return new BaseResponseDTO
        //                 {
        //                     Flag = false,
        //                     Message = "Failed to create shop"
        //                 };
        //             }
        //             catch (Exception ex)
        //             {
        //                 return new BaseResponseDTO
        //                 {
        //                     Flag = false,
        //                     Message = $"Internal server error! {ex}"
        //                 };
        //             }
        //         }
        //     }
        //     else
        //     {
        //         return new BaseResponseDTO
        //         {
        //             Flag = false,
        //             Message = "Internal server error!"
        //         };
        //     }

        // }
        // public async Task<BaseResponseDTO> DeleteShop()
        // {
        //     //check weather user is the owner of the shop
        //     var currentUserResponse = await userAccountRepository.GetMe();
        //     if (currentUserResponse.Flag == false)
        //     {
        //         return new BaseResponseDTO
        //         {
        //             Flag = false,
        //             Message = "owner cannot found"
        //         };
        //     }

        //     var currentUser = currentUserResponse.Data;

        //     using (var scope = serviceScopeFactory.CreateScope())
        //     {
        //         ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        //         try
        //         {
        //             //check whether any service provider is associated with the shop
        //             if (dbContext.Services.Where(s => s.ShopId == currentUser.Id).Any())
        //             {
        //                 return new BaseResponseDTO
        //                 {
        //                     Flag = false,
        //                     Message = "Shop cannot be deleted because it has service providers"
        //                 };
        //             }
        //             var shop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstAsync();

        //             //check if the shop exists
        //             if (shop == null)
        //             {
        //                 return new BaseResponseDTO
        //                 {
        //                     Flag = false,
        //                     Message = "Shop not found!"
        //                 };
        //             }

        //             //get all the services associated with the shop
        //             var services = await dbContext.Services.Where(s => s.ShopId == shop.Id).ToListAsync();
        //             foreach (var service in services)
        //             {
        //                 dbContext.Services.Remove(service);
        //             }

        //             //delete the shop
        //             dbContext.Shops.Remove(shop);
        //             await dbContext.SaveChangesAsync();

        //             return new BaseResponseDTO
        //             {
        //                 Flag = true,
        //                 Message = "Shop deleted successfully"
        //             };
        //         }
        //         catch (Exception ex)
        //         {
        //             return new BaseResponseDTO
        //             {
        //                 Flag = false,
        //                 Message = $"Internal server error! {ex}"
        //             };
        //         }
        //     }
        // }
        // public async Task<BaseResponseDTO<Shop>> GetMyShop()
        // {
        //     //get the current login user(owner of the shop)
        //     var currentUserResponse = await userAccountRepository.GetMe();
        //     if (currentUserResponse.Flag == false)
        //     {
        //         return new BaseResponseDTO<Shop>
        //         {
        //             Flag = false,
        //             Message = "owner cannot found"
        //         };
        //     }
        //     try
        //     {
        //         using (var scope = serviceScopeFactory.CreateScope())
        //         {
        //             ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        //             var currentUser = currentUserResponse.Data;
        //             var shop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstAsync();
        //             if (shop == null)
        //             {
        //                 return new BaseResponseDTO<Shop>
        //                 {
        //                     Flag = false,
        //                     Message = "Shop not found!"
        //                 };
        //             }

        //             //get all the services associated with the shop
        //             var services = await dbContext.Services.Where(s => s.ShopId == shop.Id).ToListAsync();

        //             //TODO: assign image url to each service
        //             // foreach (var service in services)
        //             // {
        //             //     service.ImageUrl = cloudProvider.GeneratePreSignedUrlForDownload(service.ImageUrl);
        //             // }
        //             shop.Services = services;
        //             return new BaseResponseDTO<Shop>
        //             {
        //                 Flag = true,
        //                 Message = "Shop found successfully",
        //                 Data = shop
        //             };

        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return new BaseResponseDTO<Shop>
        //         {
        //             Flag = false,
        //             Message = $"Internal server error! {ex}"
        //         };
        //     }
        // }
        // public async Task<BaseResponseDTO> UpdateShop(ShopRequestDTO shopRequestDTO)
        // {
        //     //get the current login user(owner of the shop)
        //     var currentUserResponse = await userAccountRepository.GetMe();
        //     if (currentUserResponse.Flag == false)
        //     {
        //         return new BaseResponseDTO
        //         {
        //             Flag = false,
        //             Message = "owner cannot found"
        //         };
        //     }
        //     try
        //     {
        //         using (var scope = serviceScopeFactory.CreateScope())
        //         {
        //             ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        //             var currentUser = currentUserResponse.Data;
        //             var shop = await dbContext.Shops.Where(s => s.OwnerId == currentUser.Id).FirstAsync();
        //             if (shop == null)
        //             {
        //                 return new BaseResponseDTO
        //                 {
        //                     Flag = false,
        //                     Message = "Shop not found!"
        //                 };
        //             }
        //             shop.Name = shopRequestDTO.Name;
        //             shop.Description = shopRequestDTO.Description;
        //             dbContext.Shops.Update(shop);
        //             await dbContext.SaveChangesAsync();

        //             return new BaseResponseDTO
        //             {
        //                 Flag = true,
        //                 Message = "Shop updated successfully"
        //             };
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return new BaseResponseDTO
        //         {
        //             Flag = false,
        //             Message = $"Internal server error! {ex}"
        //         };
        //     }
        // }
        // public async Task<BaseResponseDTO<List<Shop>>> GetAllShops()
        // {
        //     using (var scope = serviceScopeFactory.CreateScope())
        //     {
        //         ApplicationDbContext? dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        //         try
        //         {
        //             var shops = await dbContext.Shops.ToListAsync();
        //             if (shops.Count == 0)
        //             {
        //                 return new BaseResponseDTO<List<Shop>>
        //                 {
        //                     Flag = false,
        //                     Message = "No shops found!"
        //                 };
        //             }
        //             return new BaseResponseDTO<List<Shop>>
        //             {
        //                 Flag = true,
        //                 Message = "Shops found successfully",
        //                 Data = shops
        //             };
        //         }
        //         catch (Exception ex)
        //         {
        //             return new BaseResponseDTO<List<Shop>>
        //             {
        //                 Flag = false,
        //                 Message = $"Internal server error! {ex}"
        //             };
        //         }
        //     }
        // }
        public Task<BaseResponseDTO> CreateAsync(Shop entity)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BaseResponseDTO<Shop>>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<Shop>> FindByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<Shop>> GetShopByServiceId(string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> UpdateAsync(Shop entity)
        {
            throw new NotImplementedException();
        }
    }
}
