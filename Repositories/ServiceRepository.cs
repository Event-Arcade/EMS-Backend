using Contracts;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class ServiceRepository(IUserAccountRepository userAccountRepository, IFeedbackRepository feedBackRepository,
    IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, IConfiguration configuration) : IServiceRepository
    {
        public async Task<BaseResponseDTO> CreateAsync(Service entity)
        {
            try
            {
                //check if entity is null 
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    //assing new id into entity
                    entity.Id = Guid.NewGuid().ToString();

                    if (entity.formFiles != null)
                    {
                        entity.StaticResourcesPaths = new List<ServiceStaticResources>();
                        foreach (var file in entity.formFiles)
                        {
                            var (flag, path) = await cloudProvider.UploadFile(file, configuration["StorageDirectories:FeedbackImages"]);
                            if (!flag)
                            {
                                throw new Exception("Error uploading file");
                            }
                            var staticResource = new ServiceStaticResources
                            {
                                Id = Guid.NewGuid().ToString(),
                                ServiceId = entity.Id,
                                ResourceUrl = path
                            };

                            //add into database
                            await context.ServiceStaticResources.AddAsync(staticResource);
                        }
                    }

                    await context.Services.AddAsync(entity);
                    await context.SaveChangesAsync();
                    return new BaseResponseDTO
                    {
                        Message = "Service created successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO> DeleteAsync(string id)
        {
            try
            {
                //check if id in null
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var service = await context.Services.FindAsync(id);
                    if (service == null)
                    {
                        throw new Exception("Service not found");
                    }

                    //check any sub package exist
                    if (context.SubPackages.Any(x => x.ServiceId == id))
                    {
                        throw new Exception("Service can't be deleted because it has sub packages");
                    }

                    //delete all static resources
                    var staticResources = await context.ServiceStaticResources.Where(x => x.ServiceId == id).ToListAsync();
                    foreach (var item in staticResources)
                    {
                        //delete file from cloud
                        await cloudProvider.RemoveFile(item.ResourceUrl);

                        //remove from database
                        context.ServiceStaticResources.Remove(item);
                    }

                    //delete all feedbacks associated with service
                    var feedbacks = await context.FeedBacks.Where(x => x.ServiceId == id).ToListAsync();
                    foreach (var item in feedbacks)
                    {
                        await feedBackRepository.DeleteAsync(item.Id);
                    }

                    context.Services.Remove(service);
                    await context.SaveChangesAsync();
                    return new BaseResponseDTO
                    {
                        Message = "Service deleted successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<IEnumerable<Service>>> FindAllAsync()
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var services = await context.Services.Include(x => x.StaticResourcesPaths).Include(x => x.FeedBacks).ToListAsync();

                    //get all feedbacks for each service
                    foreach (var service in services)
                    {
                        var feedbacksForService = await feedBackRepository.GetFeedBacksByServiceId(service.Id);
                        if (feedbacksForService.Flag)
                        {
                            foreach (var feedback in feedbacksForService.Data)
                            {
                                service.FeedBacks.Add(feedback);

                            }
                        }
                    }

                    //get all static resources for each service
                    foreach (var s in services)
                    {
                        var staticResourcesForService = await context.ServiceStaticResources.Where(x => x.ServiceId == s.Id).ToListAsync();
                        //assign url to each static resource
                        foreach (var item in staticResourcesForService)
                        {
                            item.ResourceUrl = cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl);
                        }
                        s.StaticResourcesPaths = staticResourcesForService;
                    }

                    return new BaseResponseDTO<IEnumerable<Service>>
                    {
                        Data = services,
                        Message = "Services fetched successfully",
                        Flag = true
                    };

                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<Service>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<Service>> FindByIdAsync(string id)
        {
            try
            {
                //check if id is null
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var service = context.Services.Include(x => x.StaticResourcesPaths).Include(x => x.FeedBacks).FirstOrDefault(x => x.Id == id);

                    //get all feedbacks for service
                    var feedbacksForService = feedBackRepository.GetFeedBacksByServiceId(id);
                    if (feedbacksForService.Result.Flag)
                    {
                        foreach (var feedback in feedbacksForService.Result.Data)
                        {
                            service.FeedBacks.Add(feedback);
                        }
                    }

                    //get all static resources for service
                    var staticResourcesForService = context.ServiceStaticResources.Where(x => x.ServiceId == id).ToList();
                    //assign url to each static resource
                    foreach (var item in staticResourcesForService)
                    {
                        item.ResourceUrl = cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl);
                    }
                    service.StaticResourcesPaths = staticResourcesForService;

                    return new BaseResponseDTO<Service>
                    {
                        Data = service,
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<Service>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<IEnumerable<Service>>> GetServicesByShopId(string shopId)
        {
            try
            {
                //check if shopId is null
                if (shopId == null)
                {
                    throw new ArgumentNullException(nameof(shopId));
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var services = context.Services.Where(x => x.ShopId == shopId).Include(x => x.StaticResourcesPaths).Include(x => x.FeedBacks).ToList();

                    //get all feedbacks for each service
                    foreach (var service in services)
                    {
                        var feedbacksForService = feedBackRepository.GetFeedBacksByServiceId(service.Id);
                        if (feedbacksForService.Result.Flag)
                        {
                            foreach (var feedback in feedbacksForService.Result.Data)
                            {
                                service.FeedBacks.Add(feedback);
                            }
                        }
                    }

                    //get all static resources for each service
                    foreach (var s in services)
                    {
                        var staticResourcesForService = context.ServiceStaticResources.Where(x => x.ServiceId == s.Id).ToList();
                        //assign url to each static resource
                        foreach (var item in staticResourcesForService)
                        {
                            item.ResourceUrl = cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl);
                        }
                        s.StaticResourcesPaths = staticResourcesForService;
                    }

                    return new BaseResponseDTO<IEnumerable<Service>>
                    {
                        Data = services,
                        Message = "Services fetched successfully",
                        Flag = true
                    };

                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<Service>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO> UpdateAsync(String id, Service entity)
        {
            try
            {
                //check if entity is null
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    //get old service
                    var oldService = await context.Services.FindAsync(entity.Id);
                    if (oldService == null)
                    {
                        throw new Exception("Service not found");
                    }

                    //update old service
                    oldService.Name = entity.Name;
                    oldService.Price = entity.Price;
                    oldService.Description = entity.Description;
                    oldService.Rating = entity.Rating;
                    oldService.ShopId = entity.ShopId;
                    oldService.CategoryId = entity.CategoryId;

                    //delete all static resources
                    var staticResources = await context.ServiceStaticResources.Where(x => x.ServiceId == entity.Id).ToListAsync();
                    foreach (var item in staticResources)
                    {
                        //delete file from cloud
                        await cloudProvider.RemoveFile(item.ResourceUrl);

                        //remove from database
                        context.ServiceStaticResources.Remove(item);
                    }

                    //add new static resources
                    if (entity.formFiles != null)
                    {
                        entity.StaticResourcesPaths = new List<ServiceStaticResources>();
                        foreach (var file in entity.formFiles)
                        {
                            var (flag, path) = await cloudProvider.UploadFile(file, configuration["StorageDirectories:FeedbackImages"]);
                            if (!flag)
                            {
                                throw new Exception("Error uploading file");
                            }
                            var staticResource = new ServiceStaticResources
                            {
                                Id = Guid.NewGuid().ToString(),
                                ServiceId = entity.Id,
                                ResourceUrl = path
                            };

                            //add into database
                            await context.ServiceStaticResources.AddAsync(staticResource);
                        }
                    }

                    await context.SaveChangesAsync();
                    return new BaseResponseDTO
                    {
                        Message = "Service updated successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
    }
}
