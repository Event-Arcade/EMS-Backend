using Contracts;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class FeedbackRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, IConfiguration configuration) : IFeedBackRepository
    {
        public async Task<BaseResponseDTO> CreateAsync(FeedBack entity)
        {
            try
            {
                // Check entity is null
                if (entity == null)
                {
                    throw new Exception("Request is null");
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // Check if feedback already exists
                    var feedback = context.FeedBacks.FirstOrDefault(x => x.ServiceId == entity.ServiceId && x.UserId == entity.UserId);
                    if (feedback != null)
                    {
                        throw new Exception("Feedback already exists");
                    }

                    // Upload feedback image to S3
                    var (flag, filePath) = await cloudProvider.UploadFile(entity.FeedbackImage, configuration["StorageDirectories:FeedbackImages"]);

                    if (flag)
                    {
                        entity.FeedbackStaticResourcePath = filePath;
                    }
                    else
                    {
                        throw new Exception("Failed to upload feedback image");
                    }
                    // Save feedback
                    await context.FeedBacks.AddAsync(entity);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Message = "Feedback created successfully",
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
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // Find feedback by id
                    var feedback = context.FeedBacks.FirstOrDefault(x => x.Id == id);
                    if (feedback == null)
                    {
                        throw new Exception("Feedback not found");
                    }

                    // Delete feedback image from S3
                    await cloudProvider.RemoveFile(feedback.FeedbackStaticResourcePath);

                    context.FeedBacks.Remove(feedback);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Message = "Feedback deleted successfully",
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
        public async Task<BaseResponseDTO<IEnumerable<FeedBack>>> FindAllAsync()
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var feedbacks = await context.FeedBacks.ToListAsync();

                    //assing urls for static resource path in each feedback
                    foreach (var feedback in feedbacks)
                    {
                        var url = cloudProvider.GeneratePreSignedUrlForDownload(feedback.FeedbackStaticResourcePath);
                        feedback.FeedbackStaticResourcePath = url;
                    }

                    return new BaseResponseDTO<IEnumerable<FeedBack>>
                    {
                        Data = feedbacks,
                        Message = "Feedbacks fetched successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<FeedBack>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<FeedBack>> FindByIdAsync(string id)
        {
            try
            {
                // Check id is null
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("Requested Id is null");
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var feedback = await context.FeedBacks.FirstOrDefaultAsync(x => x.Id == id);

                    if (feedback == null)
                    {
                        throw new Exception("Feedback not found");
                    }

                    //assing urls for static resource path in feedback
                    var url = cloudProvider.GeneratePreSignedUrlForDownload(feedback.FeedbackStaticResourcePath);
                    feedback.FeedbackStaticResourcePath = url;

                    return new BaseResponseDTO<FeedBack>
                    {
                        Data = feedback,
                        Message = "Feedback fetched successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<FeedBack>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<IEnumerable<FeedBack>>> GetFeedBacksByServiceId(string serviceId)
        {
            try
            {
                // Check shopId is null
                if (string.IsNullOrEmpty(serviceId))
                {
                    throw new Exception("Requested shopId is null");
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    //get all feedbacks for service
                    var feedbacks = await context.FeedBacks.Where(x => x.ServiceId == serviceId).ToListAsync();

                    //assing urls for static resource path in each feedback
                    foreach (var feedback in feedbacks)
                    {
                        var url = cloudProvider.GeneratePreSignedUrlForDownload(feedback.FeedbackStaticResourcePath);
                        feedback.FeedbackStaticResourcePath = url;
                    }

                    return new BaseResponseDTO<IEnumerable<FeedBack>>
                    {
                        Data = feedbacks,
                        Message = "Feedbacks fetched successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<FeedBack>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<IEnumerable<FeedBack>>> GetFeedBacksByShopId(string shopId)
        {
            try
            {
                // Check shopId is null
                if (string.IsNullOrEmpty(shopId))
                {
                    throw new Exception("Requested shopId is null");
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //get all services in this shop
                    var services = await context.Services.Where(x => x.ShopId == shopId).ToListAsync();

                    //get all feedbacks for each service
                    List<FeedBack> feedbacks = new List<FeedBack>();
                    foreach (var service in services)
                    {
                        var feedback = await context.FeedBacks.FirstOrDefaultAsync(x => x.ServiceId == service.Id);
                        if (feedback != null)
                        {
                            feedbacks.Add(feedback);
                        }
                    }

                    //assing urls for static resource path in each feedback
                    foreach (var feedback in feedbacks)
                    {
                        var url = cloudProvider.GeneratePreSignedUrlForDownload(feedback.FeedbackStaticResourcePath);
                        feedback.FeedbackStaticResourcePath = url;
                    }

                    return new BaseResponseDTO<IEnumerable<FeedBack>>
                    {
                        Data = feedbacks,
                        Message = "Feedbacks fetched successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<FeedBack>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO> UpdateAsync(FeedBack entity)
        {
            try
            {
                // Check entity is null
                if (entity == null)
                {
                    throw new Exception("Request is null");
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // Check if feedback already exists
                    var feedback = context.FeedBacks.FirstOrDefault(x => x.Id == entity.Id);
                    if (feedback == null)
                    {
                        throw new Exception("Feedback not found");
                    }

                    //Update static-resource in s3
                    var (flag, filePath) = await cloudProvider.UpdateFile(entity.FeedbackImage, configuration["StorageDirectories:FeedbackImages"], feedback.FeedbackStaticResourcePath);
                    entity.FeedbackStaticResourcePath = filePath;

                    // Update feedback
                    context.FeedBacks.Update(entity);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Message = "Feedback updated successfully",
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