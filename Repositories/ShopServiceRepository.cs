using Contracts;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.ShopService;
using EMS.BACKEND.API.Enums;
using EMS.BACKEND.API.Mappers;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class ShopServiceRepository : IShopServiceRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICloudProviderRepository _cloudProvider;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationRepository _notificationRepository;

        public ShopServiceRepository(IServiceScopeFactory serviceScopeFactory, INotificationRepository notificationRepository, ICloudProviderRepository cloudProvider, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cloudProvider = cloudProvider;
            _configuration = configuration;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponseDTO<ShopServiceResponseDTO>> CreateAsync(string userId, ShopServiceRequestDTO entity)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shopService = entity.ToShopService();
                    //add into database
                    var newService = await context.ShopServices.AddAsync(shopService);
                    await context.SaveChangesAsync();

                    // print the newService state in the console

                    if (newService.State != EntityState.Unchanged)
                    {
                        throw new Exception("Error creating service");
                    }

                    var staticResources = new List<ShopServiceStaticResources>();
                    //add static resources
                    if (entity.ShopServiceStaticResources != null && entity.ShopServiceStaticResources.Count > 0)
                    {
                        foreach (var item in entity.ShopServiceStaticResources)
                        {
                            // upload to the cloud
                            if (item.ContentType.Contains("image"))
                            {
                                var (flag, path) = await _cloudProvider.UploadFile(item, _configuration["StorageDirectories:ServiceImages"]);
                                if (!flag)
                                {
                                    throw new Exception("Error uploading file");
                                }
                                var staticResource = new ShopServiceStaticResources
                                {
                                    ServiceId = newService.Entity.Id,
                                    ResourceUrl = path
                                };
                                //add into database
                                var response = await context.ShopServiceStaticResources.AddAsync(staticResource);
                                staticResources.Add(response.Entity);

                            }
                            else if (item.ContentType.Contains("video"))
                            {
                                var (flag, path) = await _cloudProvider.UploadFile(item, _configuration["StorageDirectories:ServiceVideos"]);
                                if (!flag)
                                {
                                    throw new Exception("Error uploading file");
                                }
                                var staticResource = new ShopServiceStaticResources
                                {
                                    ServiceId = newService.Entity.Id,
                                    ResourceUrl = path
                                };
                                //add into database
                                var response = await context.ShopServiceStaticResources.AddAsync(staticResource);
                                staticResources.Add(response.Entity);
                            }
                            else
                            {
                                throw new Exception("Invalid file type");
                            }
                        }
                    }

                    newService.Entity.ShopServiceStaticResources = staticResources;

                    //Update service
                    context.Update(newService.Entity);
                    await context.SaveChangesAsync();

                    // convert resources into URLs
                    var staticResourcesURLs = new List<string>();
                    foreach (var item in staticResources)
                    {
                        staticResourcesURLs.Add(_cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl));
                    }

                    // notify all users and admins
                    await _notificationRepository.AddNotification("New Service", $"New service {newService.Entity.Name} has been added",DatabaseChangeEventType.Add, "client", null, EntityType.Service, newService.Entity.Id, null);
                    await _notificationRepository.AddNotification("New Service", $"New service {newService.Entity.Name} has been added",DatabaseChangeEventType.Add, "admin", null, EntityType.Service, newService.Entity.Id, null);

                    // send database change event to all 
                    await _notificationRepository.SendDatabaseChangeNotification( DatabaseChangeEventType.Add, EntityType.Service, newService.Entity.Id, userId);

                    return new BaseResponseDTO<ShopServiceResponseDTO>
                    {
                        Message = "Service created successfully",
                        Flag = true,
                        Data = newService.Entity.ToShopServiceResponseDTO(userId, staticResourcesURLs)
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ShopServiceResponseDTO>
                {
                    Message = ex?.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO> DeleteAsync(string userId, int id)
        {
            try
            {
                // delete the shopService
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shopService = await context.ShopServices.Include(s => s.Shop).FirstOrDefaultAsync(x => x.Id == id);
                    if (shopService == null)
                    {
                        throw new Exception("Service not found");
                    }

                    // check if user is vendor role
                    var user = await _userManager.FindByIdAsync(userId);
                    var userRole = await _userManager.GetRolesAsync(user);
                    if (!userRole.Contains("vendor"))
                    {
                        throw new Exception("You are not authorized to delete service");
                    }

                    // check if user is owner of the shop
                    if (shopService.Shop.OwnerId != userId)
                    {
                        throw new Exception("You are not authorized to delete service for this shop");
                    }

                    //check any sub package exist
                    if (context.SubPackages.Any(x => x.ServiceId == id))
                    {
                        throw new Exception("Service can't be deleted because it has sub packages");
                    }

                    //delete all static resources
                    var staticResources = await context.ShopServiceStaticResources.Where(x => x.ServiceId == id).ToListAsync();
                    foreach (var item in staticResources)
                    {
                        //delete file from cloud
                        await _cloudProvider.RemoveFile(item.ResourceUrl);

                        //remove from database
                        context.ShopServiceStaticResources.Remove(item);
                    }

                    //delete all feedbacks associated with service
                    var feedbacks = await context.FeedBacks.Where(x => x.ServiceId == id).ToListAsync();
                    foreach (var item in feedbacks)
                    {
                        //delete all static resources associated with feedback
                        var feedbackStaticResources = await context.FeedBackStaticResources.Where(x => x.FeedBackId == item.Id).ToListAsync();
                        foreach (var resource in feedbackStaticResources)
                        {
                            //delete file from cloud
                            await _cloudProvider.RemoveFile(resource.ResourceUrl);

                            //remove from database
                            context.FeedBackStaticResources.Remove(resource);
                        }

                        //remove feedback from database
                        context.FeedBacks.Remove(item);
                    }

                    var shopServiceName = shopService.Name;

                    context.ShopServices.Remove(shopService);
                    await context.SaveChangesAsync();

                    // notify all users and admins
                    await _notificationRepository.AddNotification("Service Deleted", $"Service {shopServiceName} has been deleted",DatabaseChangeEventType.Delete, "admin", null, EntityType.Service, id, null);

                    // send database change event to all admins, clients and vendors
                    await _notificationRepository.SendDatabaseChangeNotification( DatabaseChangeEventType.Delete, EntityType.Service, id, userId);

                    return new BaseResponseDTO
                    {
                        Message = "Service deleted successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception e)
            {
                return new BaseResponseDTO
                {
                    Message = e?.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>> FindAllAsync()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var services = await context.ShopServices.ToListAsync();

                    var response = new List<ShopServiceResponseDTO>();
                    foreach (var service in services)
                    {
                        var shop = await context.Shops.Where(s=>s.Id== service.ShopId).FirstOrDefaultAsync();
                        var staticResources = await context.ShopServiceStaticResources.Where(x => x.ServiceId == service.Id).ToListAsync();
                        var staticResourcesURLs = new List<string>();
                        foreach (var item in staticResources)
                        {
                            staticResourcesURLs.Add(_cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl));
                        }
                        response.Add(service.ToShopServiceResponseDTO(shop.OwnerId, staticResourcesURLs));
                    }

                    return new BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>
                    {
                        Data = response,
                        Message = "Services fetched successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<ShopServiceResponseDTO>> FindByIdAsync(int id)
        {
            // find shopservice using its id
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shopService = await context.ShopServices.FirstOrDefaultAsync(x => x.Id == id);
                    if (shopService == null)
                    {
                        throw new Exception("Service not found");
                    }

                    var staticResources = await context.ShopServiceStaticResources.Where(x => x.ServiceId == id).ToListAsync();
                    var staticResourcesURLs = new List<string>();
                    foreach (var item in staticResources)
                    {
                        staticResourcesURLs.Add(_cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl));
                    }

                    // find shop using shopservice's shopid to include ownerid in response DTO
                    var shop = await context.Shops.Where(s=>s.Id == shopService.ShopId).FirstOrDefaultAsync();

                    return new BaseResponseDTO<ShopServiceResponseDTO>
                    {
                        Data = shopService.ToShopServiceResponseDTO(shop.OwnerId , staticResourcesURLs),
                        Message = "Service fetched successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ShopServiceResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<ShopServiceResponseDTO>> UpdateAsync(string userId, int id, ShopServiceRequestDTO entity)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shopService = await context.ShopServices.Include(s => s.Shop).FirstOrDefaultAsync(x => x.Id == id);
                    if (shopService == null)
                    {
                        throw new Exception("Service not found");
                    }

                    // check if user is vendor role
                    var user = await _userManager.FindByIdAsync(userId);
                    var userRole = await _userManager.GetRolesAsync(user);
                    if (!userRole.Contains("vendor"))
                    {
                        throw new Exception("You are not authorized to update service");
                    }

                    // check if user is owner of the shop
                    if (shopService.Shop.OwnerId != userId)
                    {
                        throw new Exception("You are not authorized to update service for this shop");
                    }

                    //update service details if not null
                    if (!string.IsNullOrEmpty(entity.Name))
                    {
                        shopService.Name = entity.Name;
                    }
                    if (!string.IsNullOrEmpty(entity.Description))
                    {
                        shopService.Description = entity.Description;
                    }
                    if (entity.NoOfGuests > 0)
                    {
                        shopService.NoOfGuests = entity.NoOfGuests;
                    }
                    shopService.Indoor = entity.Indoor;
                    shopService.Outdoor = entity.Outdoor;

                    //update static resources if not null
                    if (entity.ShopServiceStaticResources != null && entity.ShopServiceStaticResources.Count > 0)
                    {
                        var newStaticResources = new List<ShopServiceStaticResources>();
                        foreach (var item in entity.ShopServiceStaticResources)
                        {
                            // upload to the cloud
                            if (item.ContentType.Contains("image"))
                            {
                                var (flag, path) = await _cloudProvider.UploadFile(item, _configuration["StorageDirectories:ServiceImages"]);
                                if (!flag)
                                {
                                    throw new Exception("Error uploading file");
                                }
                                var staticResource = new ShopServiceStaticResources
                                {
                                    ServiceId = shopService.Id,
                                    ResourceUrl = path
                                };
                                //add into database
                                var response = await context.ShopServiceStaticResources.AddAsync(staticResource);
                                newStaticResources.Add(response.Entity);

                            }
                            else if (item.ContentType.Contains("video"))
                            {
                                var (flag, path) = await _cloudProvider.UploadFile(item, _configuration["StorageDirectories:ServiceVideos"]);
                                if (!flag)
                                {
                                    throw new Exception("Error uploading file");
                                }
                                var staticResource = new ShopServiceStaticResources
                                {
                                    ServiceId = shopService.Id,
                                    ResourceUrl = path
                                };
                                //add into database
                                var response = await context.ShopServiceStaticResources.AddAsync(staticResource);
                                newStaticResources.Add(response.Entity);
                            }
                            else
                            {
                                throw new Exception("Invalid file type");
                            }
                        }

                        //delete all static resources
                        var staticResources = await context.ShopServiceStaticResources.Where(x => x.ServiceId == shopService.Id).ToListAsync();
                        foreach (var item in staticResources)
                        {
                            //delete file from cloud
                            await _cloudProvider.RemoveFile(item.ResourceUrl);

                            //remove from database
                            context.ShopServiceStaticResources.Remove(item);
                        }

                        //Update service static resources
                        shopService.ShopServiceStaticResources = newStaticResources;
                    }

                    //Update service
                    context.Update(shopService);
                    await context.SaveChangesAsync();

                    // convert resources into URLs
                    var staticResourcesURLs = new List<string>();
                    var updatedStaticResources = await context.ShopServiceStaticResources.Where(x => x.ServiceId == shopService.Id).ToListAsync();
                    if (updatedStaticResources != null)
                    {
                        foreach (var item in updatedStaticResources)
                        {
                            staticResourcesURLs.Add(_cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl));
                        }

                        // send database change event to all admins, clients and vendors
                        await _notificationRepository.SendDatabaseChangeNotification( DatabaseChangeEventType.Update, EntityType.Service, shopService.Id, userId);

                        return new BaseResponseDTO<ShopServiceResponseDTO>
                        {
                            Message = "Service updated successfully",
                            Flag = true,
                            Data = shopService.ToShopServiceResponseDTO(userId, staticResourcesURLs)
                        };
                    }
                    else
                    {
                        // send database change event to all admins, clients and vendors
                        await _notificationRepository.SendDatabaseChangeNotification( DatabaseChangeEventType.Update, EntityType.Service, shopService.Id, userId);

                        return new BaseResponseDTO<ShopServiceResponseDTO>
                        {
                            Message = "Service updated successfully",
                            Flag = true,
                            Data = shopService.ToShopServiceResponseDTO(userId, null)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ShopServiceResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }

        }
        
        public async Task<BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>> FindByShopIdAsync(int shopId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var shop = await context.Shops.Where(x => x.Id == shopId).FirstOrDefaultAsync();

                    if(shop == null)
                    {
                        throw new Exception("Shop not found");
                    }

                    var services = await context.ShopServices.Where(x => x.ShopId == shopId).ToListAsync();

                    var response = new List<ShopServiceResponseDTO>();
                    foreach (var service in services)
                    {
                        var staticResources = await context.ShopServiceStaticResources.Where(x => x.ServiceId == service.Id).ToListAsync();
                        var staticResourcesURLs = new List<string>();
                        foreach (var item in staticResources)
                        {
                            staticResourcesURLs.Add(_cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl));
                        }
                        response.Add(service.ToShopServiceResponseDTO(shop.OwnerId, staticResourcesURLs));
                    }

                    return new BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>
                    {
                        Data = response,
                        Message = "Services fetched successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
    }
}
