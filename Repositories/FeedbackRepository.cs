using Contracts;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs;
using EMS.BACKEND.API.DTOs.Mappers;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Enums;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICloudProviderRepository _cloudProvider;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationRepository _notificationRepository;

        public FeedbackRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, INotificationRepository notificationRepository, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cloudProvider = cloudProvider;
            _configuration = configuration;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponseDTO<FeedBackResponseDTO>> CreateAsync(string userId, FeedBackRequestDTO entity)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var shopId = 0;
                    var shopServiceId = 0;
                    // get the user
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return new BaseResponseDTO<FeedBackResponseDTO>
                        {
                            Message = "User not found",
                            Flag = false
                        };
                    }

                    // check if the service exists
                    var service = await context.ShopServices.FirstOrDefaultAsync(x => x.Id == entity.ServiceId);
                    if (service == null)
                    {
                        return new BaseResponseDTO<FeedBackResponseDTO>
                        {
                            Message = "Service not found",
                            Flag = false
                        };
                    }

                    // check if the user has already given feedback
                    var feedBack = await context.FeedBacks.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ServiceId == entity.ServiceId);
                    if (feedBack != null)
                    {
                        return new BaseResponseDTO<FeedBackResponseDTO>
                        {
                            Message = "You have already given feedback for this service",
                            Flag = false
                        };
                    }

                    // create the feedback
                    var feedBackEntity = entity.ToFeedBack(userId);
                    shopServiceId = feedBackEntity.ServiceId;
                    var newFeedback = await context.FeedBacks.AddAsync(feedBackEntity);
                    await context.SaveChangesAsync();

                    // update the service rating according to the feedbacks rating
                    var feedbacks = await context.FeedBacks.Where(x => x.ServiceId == entity.ServiceId).ToListAsync();
                    double totalRating = 0;
                    foreach (var feedback in feedbacks)
                    {
                        totalRating += feedback.Rating;
                    }
                    service.Rating = totalRating / feedbacks.Count;
                    context.ShopServices.Update(service);
                    await context.SaveChangesAsync();

                    // update the shop rating according to the services rating
                    var shop = await context.Shops.FirstOrDefaultAsync(x => x.Id == service.ShopId);
                    shopId = shop.Id;
                    var services = await context.ShopServices.Where(x => x.ShopId == shop.Id).ToListAsync();
                    double totalShopRating = 0;
                    foreach (var s in services)
                    {
                        totalShopRating += s.Rating;
                    }
                    shop.Rating = totalShopRating / services.Count;
                    context.Shops.Update(shop);

                    if (newFeedback.State == EntityState.Added)
                    {
                        throw new Exception("Feedback not created, please try again later");
                    }

                    // upload the iformfiles and create the feedBackStaticResources
                    ICollection<FeedBackStaticResource> feedBackStaticResources = new List<FeedBackStaticResource>();
                    if (entity.FeedBackStaticResources != null)
                    {
                        foreach (var file in entity.FeedBackStaticResources)
                        {
                            // file shoud be type image
                            if (file.ContentType.Contains("image"))
                            {
                                var (flag, path) = await _cloudProvider.UploadFile(file, _configuration["StorageDirectories:FeedbackImages"]);
                                if (flag)
                                {
                                    var feedBackStaticResource = new FeedBackStaticResource
                                    {
                                        FeedBackId = newFeedback.Entity.Id,
                                        ResourceUrl = path
                                    };
                                    feedBackStaticResources.Add(feedBackStaticResource);
                                }
                                else
                                {
                                    throw new Exception("Error uploading file");
                                }
                            }
                        }
                    }

                    // add the feedBackStaticResources to the feedback
                    if (feedBackStaticResources.Count > 0)
                    {
                        await context.FeedBackStaticResources.AddRangeAsync(feedBackStaticResources);
                        await context.SaveChangesAsync();
                    }

                    // generate presigned urls
                    ICollection<string> feedBackStaticResourcesUrls = new List<string>();
                    foreach (var feedBackStaticResource in feedBackStaticResources)
                    {
                        var url = _cloudProvider.GeneratePreSignedUrlForDownload(feedBackStaticResource.ResourceUrl);
                        feedBackStaticResourcesUrls.Add(url);
                    }

                    // send notification to the shop owner
                    await _notificationRepository.AddNotification("New feedback", $"You have received a new feedback from {user.FirstName}", DatabaseChangeEventType.Add, null, shop.OwnerId, EntityType.Shop, entity.ServiceId, userId);

                    // send database change event to all clients, vendors and admins
                    await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Add, EntityType.Feedback, newFeedback.Entity.Id, userId);
                    await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Update, EntityType.Shop, shopId, userId);
                    await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Update, EntityType.Service, shopServiceId, userId);


                    return new BaseResponseDTO<FeedBackResponseDTO>
                    {
                        Data = newFeedback.Entity.ToFeedBackResponseDTO(feedBackStaticResourcesUrls),
                        Message = "Feedback created successfully",
                        Flag = true
                    };

                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<FeedBackResponseDTO>
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
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // get the user
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return new BaseResponseDTO
                        {
                            Message = "User not found",
                            Flag = false
                        };
                    }

                    // check if the feedback exists
                    var feedBack = await context.FeedBacks.FirstOrDefaultAsync(x => x.Id == id);
                    if (feedBack == null)
                    {
                        return new BaseResponseDTO
                        {
                            Message = "Feedback not found",
                            Flag = false
                        };
                    }

                    // check if the user is the owner of the feedback 
                    if (feedBack.ApplicationUserId != userId)
                    {
                        return new BaseResponseDTO
                        {
                            Message = "You are not the owner of the feedback",
                            Flag = false
                        };
                    }

                    // delete the feedBackStaticResources
                    var feedBackStaticResources = await context.FeedBackStaticResources.Where(x => x.FeedBackId == id).ToListAsync();
                    if (feedBackStaticResources.Count > 0)
                    {
                        foreach (var feedBackStaticResource in feedBackStaticResources)
                        {
                            await _cloudProvider.RemoveFile(feedBackStaticResource.ResourceUrl);
                        }
                        context.FeedBackStaticResources.RemoveRange(feedBackStaticResources);
                        await context.SaveChangesAsync();
                    }

                    var serviceId = feedBack.ServiceId;

                    // delete the feedback
                    context.FeedBacks.Remove(feedBack);
                    await context.SaveChangesAsync();

                    // update the service rating according to the feedbacks rating
                    var service = await context.ShopServices.FirstOrDefaultAsync(x => x.Id == serviceId);
                    if (service != null)
                    {
                        var feedbacks = await context.FeedBacks.Where(x => x.ServiceId == serviceId).ToListAsync();
                        if (feedbacks != null && feedbacks.Count > 0)
                        {
                            double totalRating = 0;
                            foreach (var feedback in feedbacks)
                            {
                                totalRating += feedback.Rating;
                            }
                            service.Rating = totalRating / feedbacks.Count;
                            context.ShopServices.Update(service);
                            await context.SaveChangesAsync();
                        }

                        // update the shop rating according to the services rating
                        var shop = await context.Shops.FirstOrDefaultAsync(x => x.Id == service.ShopId);
                        if (shop != null)
                        {
                            var shopId = shop.Id;
                            var services = await context.ShopServices.Where(x => x.ShopId == shop.Id).ToListAsync();
                            if (services != null && services.Count > 0)
                            {
                                double totalShopRating = 0;
                                foreach (var s in services)
                                {
                                    totalShopRating += s.Rating;
                                }
                                shop.Rating = totalShopRating / services.Count;
                                context.Shops.Update(shop);
                                await context.SaveChangesAsync();
                            }
                            // send notification to the shop owner
                            await _notificationRepository.AddNotification("Feedback deleted", $"Your feedback has been deleted by {user.FirstName}", DatabaseChangeEventType.Delete, null, shop.OwnerId, EntityType.Shop, feedBack.ServiceId, userId);

                            // send database change event to all clients, vendors and admins
                            await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Delete, EntityType.Feedback, id, userId);
                            await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Update, EntityType.Service, serviceId, userId);
                            await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Update, EntityType.Shop, shopId, userId);

                            return new BaseResponseDTO
                            {
                                Message = "Feedback deleted successfully",
                                Flag = true
                            };
                        }
                    }
                    throw new Exception("Error deleting feedback");
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

        public async Task<BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>> FindAllAsync()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var feedBacks = await context.FeedBacks.ToListAsync();
                    ICollection<FeedBackResponseDTO> feedBackResponseDTOs = new List<FeedBackResponseDTO>();
                    foreach (var feedBack in feedBacks)
                    {
                        var feedBackStaticResources = await context.FeedBackStaticResources.Where(x => x.FeedBackId == feedBack.Id).ToListAsync();
                        ICollection<string> feedBackStaticResourcesUrls = new List<string>();
                        foreach (var feedBackStaticResource in feedBackStaticResources)
                        {
                            var url = _cloudProvider.GeneratePreSignedUrlForDownload(feedBackStaticResource.ResourceUrl);
                            feedBackStaticResourcesUrls.Add(url);
                        }
                        feedBackResponseDTOs.Add(feedBack.ToFeedBackResponseDTO(feedBackStaticResourcesUrls));
                    }

                    return new BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>
                    {
                        Data = feedBackResponseDTOs,
                        Message = "FeedBacks found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<FeedBackResponseDTO>> FindByIdAsync(int id)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var feedBack = await context.FeedBacks.FirstOrDefaultAsync(x => x.Id == id);
                    if (feedBack == null)
                    {
                        return new BaseResponseDTO<FeedBackResponseDTO>
                        {
                            Message = "FeedBack not found",
                            Flag = false
                        };
                    }

                    var feedBackStaticResources = await context.FeedBackStaticResources.Where(x => x.FeedBackId == feedBack.Id).ToListAsync();
                    ICollection<string> feedBackStaticResourcesUrls = new List<string>();
                    foreach (var feedBackStaticResource in feedBackStaticResources)
                    {
                        var url = _cloudProvider.GeneratePreSignedUrlForDownload(feedBackStaticResource.ResourceUrl);
                        feedBackStaticResourcesUrls.Add(url);
                    }

                    return new BaseResponseDTO<FeedBackResponseDTO>
                    {
                        Data = feedBack.ToFeedBackResponseDTO(feedBackStaticResourcesUrls),
                        Message = "FeedBack found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<FeedBackResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public Task<BaseResponseDTO<FeedBackResponseDTO>> UpdateAsync(string userId, int id, FeedBackRequestDTO entity)
        {
            throw new NotImplementedException();
        }

    }
}