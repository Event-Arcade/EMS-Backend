using Contracts;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.ShopService;
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
        private readonly IFeedbackRepository _feedBackRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShopServiceRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, IConfiguration configuration, IFeedbackRepository feedBackRepository, UserManager<ApplicationUser> userManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cloudProvider = cloudProvider;
            _configuration = configuration;
            _feedBackRepository = feedBackRepository;
            _userManager = userManager;
        }

        public Task<BaseResponseDTO<ShopServiceResponseDTO>> CreateAsync(string userId, ShopServiceRequestDTO entity)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> DeleteAsync(string userId, int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<ShopServiceResponseDTO>> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<IEnumerable<ShopService>>> GetServicesByShopId(string shopId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<ShopServiceResponseDTO>> UpdateAsync(string userId, int id, ShopServiceRequestDTO entity)
        {
            throw new NotImplementedException();
        }

        // public async Task<BaseResponseDTO<ShopServiceResponseDTO>> CreateAsync(string userId, ShopServiceRequestDTO entity)
        // {
        //     try
        //     {
        //         // check if user is vendor role
        //         var user = await _userManager.FindByIdAsync(userId);
        //         var userRole = await _userManager.GetRolesAsync(user);
        //         if (!userRole.Contains("vendor"))
        //         {
        //             throw new Exception("You are not authorized to create service");
        //         }

        //         using (var scope = _serviceScopeFactory.CreateScope())
        //         {
        //             var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //             // check user is owner of the shop
        //             var shop = await context.Shops.FirstOrDefaultAsync(x => x.OwnerId == userId);
        //             if (shop == null)
        //             {
        //                 throw new Exception("Shop not found");
        //             }
        //             if (shop.Id != entity.ShopId)
        //             {
        //                 throw new Exception("You are not authorized to create service for this shop");
        //             }
        //             var shopService = entity.ToShopService();
        //             //add into database
        //             var newService = await context.ShopServices.AddAsync(shopService);

        //             if (newService.State != EntityState.Added)
        //             {
        //                 throw new Exception("Error creating service");
        //             }

        //             var staticResources = new List<ShopServiceStaticResources>();
        //             //add static resources
        //             if (entity.ShopServiceStaticResources.Count > 0)
        //             {
        //                 foreach (var item in entity.ShopServiceStaticResources)
        //                 {
        //                     // upload to the cloud
        //                     if (item.ContentType.Contains("image"))
        //                     {
        //                         var (flag, path) = await _cloudProvider.UploadFile(item, _configuration["StorageDirectories:ServiceImages"]);
        //                         if (!flag)
        //                         {
        //                             throw new Exception("Error uploading file");
        //                         }
        //                         var staticResource = new ShopServiceStaticResources
        //                         {
        //                             ServiceId = newService.Entity.Id,
        //                             ResourceUrl = path
        //                         };
        //                         //add into database
        //                         var response = await context.ShopServiceStaticResources.AddAsync(staticResource);
        //                         staticResources.Add(response.Entity);

        //                     }
        //                     else if (item.ContentType.Contains("video"))
        //                     {
        //                         var (flag, path) = await _cloudProvider.UploadFile(item, _configuration["StorageDirectories:ServiceVideos"]);
        //                         if (!flag)
        //                         {
        //                             throw new Exception("Error uploading file");
        //                         }
        //                         var staticResource = new ShopServiceStaticResources
        //                         {
        //                             ServiceId = newService.Entity.Id,
        //                             ResourceUrl = path
        //                         };
        //                         //add into database
        //                         var response = await context.ShopServiceStaticResources.AddAsync(staticResource);
        //                         staticResources.Add(response.Entity);
        //                     }
        //                     else
        //                     {
        //                         throw new Exception("Invalid file type");
        //                     }
        //                 }
        //             }

        //             newService.Entity.ShopServiceStaticResources = staticResources;

        //             //Update service
        //             context.Update(newService.Entity);
        //             await context.SaveChangesAsync();

        //             // convert resources into URLs
        //             var staticResourcesURLs = new List<string>();
        //             foreach (var item in staticResources)
        //             {
        //                 staticResourcesURLs.Add(_cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl));
        //             }

        //             return new BaseResponseDTO<ShopServiceResponseDTO>
        //             {
        //                 Message = "Service created successfully",
        //                 Flag = true,
        //                 Data = newService.Entity.ToShopServiceResponseDTO(staticResourcesURLs)
        //             };
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return new BaseResponseDTO<ShopServiceResponseDTO>
        //         {
        //             Message = ex.Message,
        //             Flag = false
        //         };
        //     }
        // }
        // public async Task<BaseResponseDTO> DeleteAsync(string userId, int id)
        // {
        //     try
        //     {
        //         using (var scope = _serviceScopeFactory.CreateScope())
        //         {
        //             var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //             var service = await context.ShopServices.Include(s => s.Shop).FirstOrDefaultAsync(x => x.Id == id);
        //             if (service == null)
        //             {
        //                 throw new Exception("Service not found");
        //             }

        //             // check if user is vendor role
        //             var user = await _userManager.FindByIdAsync(userId);
        //             var userRole = await _userManager.GetRolesAsync(user);
        //             if (!userRole.Contains("vendor"))
        //             {
        //                 throw new Exception("You are not authorized to delete service");
        //             }

        //             // check if user is owner of the shop
        //             if (service.Shop.OwnerId != userId)
        //             {
        //                 throw new Exception("You are not authorized to delete service for this shop");
        //             }

        //             //check any sub package exist
        //             if (context.SubPackages.Any(x => x.ServiceId == id))
        //             {
        //                 throw new Exception("Service can't be deleted because it has sub packages");
        //             }

        //             //delete all static resources
        //             var staticResources = await context.ShopServiceStaticResources.Where(x => x.ServiceId == id).ToListAsync();
        //             foreach (var item in staticResources)
        //             {
        //                 //delete file from cloud
        //                 await _cloudProvider.RemoveFile(item.ResourceUrl);

        //                 //remove from database
        //                 context.ShopServiceStaticResources.Remove(item);
        //             }

        //             //delete all feedbacks associated with service
        //             var feedbacks = await context.FeedBacks.Where(x => x.ServiceId == id).ToListAsync();
        //             foreach (var item in feedbacks)
        //             {
        //                 await _feedBackRepository.DeleteAsync(user.Id, item.Id);
        //             }

        //             context.ShopServices.Remove(service);
        //             await context.SaveChangesAsync();
        //             return new BaseResponseDTO
        //             {
        //                 Message = "Service deleted successfully",
        //                 Flag = true
        //             };
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return new BaseResponseDTO
        //         {
        //             Message = ex.Message,
        //             Flag = false
        //         };
        //     }
        // }

        // public Task<BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>> FindAllAsync()
        // {
        //     throw new NotImplementedException();
        // }

        // // public async Task<BaseResponseDTO<IEnumerable<ShopServiceResponseDTO>>> FindAllAsync()
        // // {
        // //     try
        // //     {
        // //         using (var scope = _serviceScopeFactory.CreateScope())
        // //         {
        // //             var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // //             var services = await context.Services.Include(x => x.FormFiles).Include(x => x.FormFiles).ToListAsync();

        // //             //get all feedbacks for each service
        // //             foreach (var service in services)
        // //             {
        // //                 var feedbacksForService = await _feedBackRepository.GetFeedBacksByServiceId(service.Id);
        // //                 if (feedbacksForService.Flag)
        // //                 {
        // //                     // foreach (var feedback in feedbacksForService.Data)
        // //                     // {
        // //                     //     service.FeedBacks.Add(feedback);

        // //                     // }
        // //                 }
        // //             }

        // //             //get all static resources for each service
        // //             foreach (var s in services)
        // //             {
        // //                 var staticResourcesForService = await context.ServiceStaticResources.Where(x => x.ServiceId == s.Id).ToListAsync();
        // //                 //assign url to each static resource
        // //                 foreach (var item in staticResourcesForService)
        // //                 {
        // //                     item.ResourceUrl = _cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl);
        // //                 }
        // //                 // s.StaticResourcesPaths = staticResourcesForService;
        // //             }

        // //             return new BaseResponseDTO<IEnumerable<Service>>
        // //             {
        // //                 Data = services,
        // //                 Message = "Services fetched successfully",
        // //                 Flag = true
        // //             };

        // //         }
        // //     }
        // //     catch (Exception ex)
        // //     {
        // //         return new BaseResponseDTO<IEnumerable<Service>>
        // //         {
        // //             Message = ex.Message,
        // //             Flag = false
        // //         };
        // //     }
        // // }
        // public async Task<BaseResponseDTO<ShopServiceResponseDTO>> FindByIdAsync(string id)
        // {
        //     try
        //     {
        //         //check if id is null
        //         if (id == null)
        //         {
        //             throw new ArgumentNullException(nameof(id));
        //         }

        //         using (var scope = _serviceScopeFactory.CreateScope())
        //         {
        //             var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //             //var service = context.Services.Include(x => x.StaticResourcesPaths).Include(x => x.FeedBacks).FirstOrDefault(x => x.Id == id);

        //             //get all feedbacks for service
        //             var feedbacksForService = _feedBackRepository.GetFeedBacksByServiceId(id);
        //             if (feedbacksForService.Result.Flag)
        //             {
        //                 // foreach (var feedback in feedbacksForService.Result.Data)
        //                 // {
        //                 //     service.FeedBacks.Add(feedback);
        //                 // }
        //             }

        //             //get all static resources for service
        //             var staticResourcesForService = context.ServiceStaticResources.Where(x => x.ServiceId == id).ToList();
        //             //assign url to each static resource
        //             foreach (var item in staticResourcesForService)
        //             {
        //                 item.ResourceUrl = _cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl);
        //             }
        //             //service.StaticResourcesPaths = staticResourcesForService;

        //             return new BaseResponseDTO<Service>
        //             {
        //                 //Data = service,
        //                 Flag = true
        //             };
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return new BaseResponseDTO<Service>
        //         {
        //             Message = ex.Message,
        //             Flag = false
        //         };
        //     }
        // }


        // public async Task<BaseResponseDTO<IEnumerable<ShopService>>> GetServicesByShopId(string shopId)
        // {
        //     try
        //     {
        //         //check if shopId is null
        //         if (shopId == null)
        //         {
        //             throw new ArgumentNullException(nameof(shopId));
        //         }

        //         using (var scope = _serviceScopeFactory.CreateScope())
        //         {
        //             var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //             var services = context.Services.Where(x => x.ShopId == shopId).Include(x => x.FormFiles).Include(x => x.FormFiles).ToList();

        //             //get all feedbacks for each service
        //             foreach (var service in services)
        //             {
        //                 var feedbacksForService = _feedBackRepository.GetFeedBacksByServiceId(service.Id);
        //                 if (feedbacksForService.Result.Flag)
        //                 {
        //                     foreach (var feedback in feedbacksForService.Result.Data)
        //                     {
        //                         //service.FeedBacks.Add(feedback);
        //                     }
        //                 }
        //             }

        //             //get all static resources for each service
        //             foreach (var s in services)
        //             {
        //                 var staticResourcesForService = context.ServiceStaticResources.Where(x => x.ServiceId == s.Id).ToList();
        //                 //assign url to each static resource
        //                 foreach (var item in staticResourcesForService)
        //                 {
        //                     item.ResourceUrl = _cloudProvider.GeneratePreSignedUrlForDownload(item.ResourceUrl);
        //                 }
        //                 //s.StaticResourcesPaths = staticResourcesForService;
        //             }

        //             return new BaseResponseDTO<IEnumerable<Service>>
        //             {
        //                 Data = services,
        //                 Message = "Services fetched successfully",
        //                 Flag = true
        //             };

        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return new BaseResponseDTO<IEnumerable<Service>>
        //         {
        //             Message = ex.Message,
        //             Flag = false
        //         };
        //     }
        // }


    }
}
