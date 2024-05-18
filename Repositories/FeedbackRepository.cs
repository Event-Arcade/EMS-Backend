using Contracts;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs;
using EMS.BACKEND.API.DTOs.Mappers;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
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

        public FeedbackRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, IConfiguration configuration,  UserManager<ApplicationUser> userManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cloudProvider = cloudProvider;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<BaseResponseDTO<FeedBackResponseDTO>> CreateAsync(string userId, FeedBackRequestDTO entity)
        {
            try{
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
                    //feedBackEntity.User = user;
                    //feedBackEntity.Service = service;
                    var newFeedback = await context.FeedBacks.AddAsync(feedBackEntity);
                    await context.SaveChangesAsync();

                    if(newFeedback.State == EntityState.Added)
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
                                }else{
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

                    // delete the feedback
                    context.FeedBacks.Remove(feedBack);
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

        public async Task<BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>> GetFeedBacksByServiceId(string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>> GetFeedBacksByShopId(string shopId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<FeedBackResponseDTO>> UpdateAsync(string userId, int id, FeedBackRequestDTO entity)
        {
            throw new NotImplementedException();
        }

    }
}