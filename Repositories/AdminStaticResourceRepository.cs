using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.AdminStaticResource;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Mappers;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Controllers
{
    public class AdminStaticResourceRepository : IAdminStaticResourceRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICloudProviderRepository _cloudProvider;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminStaticResourceRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cloudProvider = cloudProvider;
            _configuration = configuration;
            _userManager = userManager;
        }


        public async Task<BaseResponseDTO<AdminStaticResourceResponseDTO>> CreateAsync(string userId, AdminStaticResourceRequestDTO entity)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                    {
                        Message = "User not found",
                        Flag = false
                    };
                }

                // check user is admin
                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                    {
                        Message = "User is not an admin",
                        Flag = false
                    };
                }

                var adminStaticResource = entity.ToAdminStaticResource(userId);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var newStaticResource = await context.AdminStaticResources.AddAsync(adminStaticResource);
                    await context.SaveChangesAsync();

                    var (flag, path) = await _cloudProvider.UploadFile(entity.Resource, _configuration["StorageDirectories:AdminStaticResources"]);

                    if (flag)
                    {
                        newStaticResource.Entity.ResourceUrl = path;
                        await context.SaveChangesAsync();
                    }

                    adminStaticResource.ResourceUrl = path;
                    context.AdminStaticResources.Update(adminStaticResource);
                    await context.SaveChangesAsync();

                    // generate presigned url
                    var url = _cloudProvider.GeneratePreSignedUrlForDownload(path);

                    return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                    {
                        Data = adminStaticResource.ToAdminStaticResourceResponseDTO(url),
                        Message = "Resource created successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<AdminStaticResourceResponseDTO>
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
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new BaseResponseDTO
                    {
                        Message = "User not found",
                        Flag = false
                    };
                }

                // check user is admin
                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    return new BaseResponseDTO
                    {
                        Message = "User is not an admin",
                        Flag = false
                    };
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var staticResource = await context.AdminStaticResources.FirstOrDefaultAsync(x => x.Id == id);
                    if (staticResource == null)
                    {
                        return new BaseResponseDTO
                        {
                            Message = "Resource not found",
                            Flag = false
                        };
                    }

                    context.AdminStaticResources.Remove(staticResource);
                    await context.SaveChangesAsync();

                    // delete file from cloud
                    var flag = await _cloudProvider.RemoveFile(staticResource.ResourceUrl);
                    if (flag)
                    {
                        return new BaseResponseDTO
                        {
                            Message = "Resource deleted successfully",
                            Flag = true
                        };
                    }

                    return new BaseResponseDTO
                    {
                        Message = "Resource deleted successfully but failed to delete from cloud",
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

        public async Task<BaseResponseDTO<IEnumerable<AdminStaticResourceResponseDTO>>> FindAllAsync()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var staticResources = await context.AdminStaticResources.ToListAsync();
                    if (staticResources.Count == 0)
                    {
                        return new BaseResponseDTO<IEnumerable<AdminStaticResourceResponseDTO>>
                        {
                            Message = "No resources found",
                            Flag = false
                        };
                    }

                    var response = new List<AdminStaticResourceResponseDTO>();
                    foreach (var staticResource in staticResources)
                    {
                        // generate presigned url
                        var url = _cloudProvider.GeneratePreSignedUrlForDownload(staticResource.ResourceUrl);
                        response.Add(staticResource.ToAdminStaticResourceResponseDTO(url));
                    }

                    return new BaseResponseDTO<IEnumerable<AdminStaticResourceResponseDTO>>
                    {
                        Data = response,
                        Message = "Resources found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<AdminStaticResourceResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<AdminStaticResourceResponseDTO>> FindByIdAsync(int id)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var staticResource = await context.AdminStaticResources.FirstOrDefaultAsync(x => x.Id == id);
                    if (staticResource == null)
                    {
                        return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                        {
                            Message = "Resource not found",
                            Flag = false
                        };
                    }

                    // generate presigned url
                    var url = _cloudProvider.GeneratePreSignedUrlForDownload(staticResource.ResourceUrl);

                    return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                    {
                        Data = staticResource.ToAdminStaticResourceResponseDTO(url),
                        Message = "Resource found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };

            }
        }

        public async Task<BaseResponseDTO<AdminStaticResourceResponseDTO>> UpdateAsync(string userId, int id, AdminStaticResourceRequestDTO entity)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                    {
                        Message = "User not found",
                        Flag = false
                    };
                }

                // check user is admin
                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                    {
                        Message = "User is not an admin",
                        Flag = false
                    };
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var staticResource = await context.AdminStaticResources.FirstOrDefaultAsync(x => x.Id == id);
                    if (staticResource == null)
                    {
                        return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                        {
                            Message = "Resource not found",
                            Flag = false
                        };
                    }

                    if (entity.Resource != null)
                    {
                        var (flag, path) = await _cloudProvider.UpdateFile(entity.Resource, _configuration["StorageDirectories:AdminStaticResources"], staticResource.ResourceUrl);

                        if (flag)
                        {

                            context.AdminStaticResources.Update(staticResource);
                            if (!string.IsNullOrEmpty(path))
                            {
                                staticResource.ResourceUrl = path;
                            }
                            if (!string.IsNullOrEmpty(entity.Name))
                            {
                                staticResource.Name = entity.Name;
                            }
                            if (!string.IsNullOrEmpty(entity.Description))
                            {
                                staticResource.Description = entity.Description;
                            }

                            context.AdminStaticResources.Update(staticResource);
                            await context.SaveChangesAsync();

                            // generate presigned url
                            var url = _cloudProvider.GeneratePreSignedUrlForDownload(path);

                            return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                            {
                                Data = staticResource.ToAdminStaticResourceResponseDTO(url),
                                Message = "Resource updated successfully",
                                Flag = true
                            };
                        }
                    }

                    return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                    {
                        Message = "Failed to update resource",
                        Flag = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<AdminStaticResourceResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
    }
}