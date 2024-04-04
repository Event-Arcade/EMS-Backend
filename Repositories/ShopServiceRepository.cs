using System.Security.Claims;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class ShopServiceRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration,
                                            IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor,
                                            ICloudProviderRepository cloudProvider, IServiceRepository serviceRepository) : IShopServiceRepository
    {
        public async Task<BaseResponseDTO> CreateAsync(Shop entity)
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
                    var user = await userManager.FindByEmailAsync(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value);
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
                    return new BaseResponseDTO { Message = "Shop created successfully", Flag = true };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Message = ex.Message, Flag = false };
            }
        }

        public async Task<BaseResponseDTO> DeleteAsync(string id)
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
                    var shop = await dbContext.Shops.FindAsync(id);

                    if (shop == null)
                    {
                        return new BaseResponseDTO { Message = "Shop not found", Flag = false };
                    }

                    //check if there any services associated with the shop
                    var services = await dbContext.Services.Where(s => s.ShopId == id).ToListAsync();
                    if (services.Count > 0)
                    {
                        return new BaseResponseDTO { Message = "Shop has services associated with it", Flag = false };
                    }

                    dbContext.Shops.Remove(shop);
                    dbContext.SaveChanges();
                    return new BaseResponseDTO { Message = "Shop deleted successfully", Flag = true };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Message = ex.Message, Flag = false };
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

        public Task<BaseResponseDTO> UpdateAsync(Shop entity)
        {
            throw new NotImplementedException();
        }
    }
}
