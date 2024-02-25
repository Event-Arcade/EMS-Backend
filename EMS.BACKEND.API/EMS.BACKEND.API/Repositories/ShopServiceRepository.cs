using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Contracts;
using System.Security.Claims;
using static EMS.BACKEND.API.DTOs.ResponseDTOs.Responses;

namespace EMS.BACKEND.API.Repositories
{
    public class ShopServiceRepository(UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor, IUserAccountRepository userAccountRepository, IServiceScopeFactory serviceScopeFactory) : IShopServiceRepository
    {
        public async Task<GeneralResponse> AddNewService(ServiceRequestDTO serviceRequestDTO)
        {
            if(serviceRequestDTO == null)
            {
                return new GeneralResponse(false, "serviceRequestDTO is empty");
            }
            var newService = new Service()
            {
                Name = serviceRequestDTO.Name,
                Price = serviceRequestDTO.Price,
                Logitude = serviceRequestDTO.Longitude,
                Latitude = serviceRequestDTO.Latitude,

            };
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    var response = await dbContext.Services.AddAsync(newService);
                    var createdService = await dbContext.Services.FindAsync(newService);
                    if (createdService != null)
                    {
                        var currentShop = await dbContext.Shops.Where(s => s.Id == serviceRequestDTO.ShopId).FirstOrDefaultAsync();
                        if (currentShop != null)
                        {
                            currentShop.Services.Add(createdService);
                            await dbContext.SaveChangesAsync();
                            return new GeneralResponse(true, $"new Service is added successfully! response - {response.ToString()}");

                        }
                    }
                    return new GeneralResponse(false, "Internal server error!");
                }
                
            }catch (Exception ex)
            {
                return new GeneralResponse(false, $"error : {ex.ToString()}");
            }
           
        }

        public async Task<GeneralResponse> CreateShop(ShopRequestDTO shopRequestDTO)
        {
            if(shopRequestDTO == null)
            {
                return new GeneralResponse(false, "shopReqeustDTO is null");
            }
            var currentUserResponse = await userAccountRepository.GetMe();
            if (currentUserResponse.Flag == false)
            {
                return new GeneralResponse(false, "owner cannot found");
            }
            try
            {
                var currentUser = await userManager.FindByIdAsync(currentUserResponse.UserRequest.Id);
                var newShop = new Shop()
                    {
                        Id = shopRequestDTO.Id,
                        Name = shopRequestDTO.Name,
                        Description = shopRequestDTO.Description,
                        Rating = shopRequestDTO.Rating,
                        Owner = currentUser
                    };
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    var response = await dbContext.Shops.AddAsync(newShop);
                    await dbContext.SaveChangesAsync();
                    if (response != null)
                    {
                        var createdShop = dbContext.Shops.Where(s => s.Owner.Id == currentUser.Id).FirstAsync();
                        return new GeneralResponse(true, createdShop.Id.ToString());
                    }
                    try
                    {
                        dbContext.Shops.Add(newShop);
                        dbContext.SaveChanges();
                        if (true)
                        {
                            var createdShop = dbContext.Shops.Where(s => s.Owner.Id == currentUser.Id).FirstAsync();
                            return new GeneralResponse(true, createdShop.Id.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        return new GeneralResponse(false, $"Internal server error! {ex}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, $"Internal server error! {ex}");
            }

        }

        public Task<GeneralResponse> DeleteService(ServiceRequestDTO serviceRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> DeleteShop(ShopRequestDTO shopRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetService(ServiceRequestDTO serviceRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> GetShop()
        {
            var currentUser = await userAccountRepository.GetMe();
            if (currentUser == null)
            {
                return new GeneralResponse(true, currentUser.UserRequest.Id);
            }
            return new GeneralResponse(false, currentUser.Message);
        }

        public Task<GeneralResponse> UpdateService(ServiceRequestDTO serviceRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> UpdateShop(ShopRequestDTO shopRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
