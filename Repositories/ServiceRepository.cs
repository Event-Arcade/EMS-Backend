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
    public class ServiceRepository(UserManager<ApplicationUser> userManager, IUserAccountRepository userAccountRepository,
                                            IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor) : IServiceRepository
    {
        public async Task<BaseResponseDTO> Create(ServiceRequestDTO serviceRequestDTO)
        {
            if (serviceRequestDTO == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "ServiceRequestDTO is null"
                };

            }

            var service = new Service
            {
                Id = Guid.NewGuid().ToString(),
                Name = serviceRequestDTO.Name,
                Price = serviceRequestDTO.Price,
                ShopId = serviceRequestDTO.ShopId
            };

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    //check if service exists
                    var serviceExists = await dbContext.Services.AnyAsync(s => s.Name == service.Name && s.ShopId == service.ShopId);
                    if (serviceExists)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "Service already exists"
                        };
                    }

                    await dbContext.Services.AddAsync(service);
                    await dbContext.SaveChangesAsync();

                    return new BaseResponseDTO<Service>
                    {
                        Flag = true,
                        Message = "Service created successfully",
                        Data = service
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = ex.Message
                    };
                }
            }
        }

        public async Task<BaseResponseDTO> Delete(string id)
        {
            //chech if id is null or empty
            if (string.IsNullOrEmpty(id))
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "Service Id is null or empty"
                };
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var service = await dbContext.Services.FirstOrDefaultAsync(s => s.Id == id);

                    //check if service exists
                    if (service == null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "Service not found"
                        };
                    }

                    dbContext.Services.Remove(service);
                    await dbContext.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Flag = true,
                        Message = "Service deleted successfully"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = ex.Message
                    };
                }
            }
        }

        public async Task<BaseResponseDTO> GetAllServices()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var services = dbContext.Services.ToList();

                    //check if services exist
                    if (services == null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "No services found"
                        };
                    }

                    var serviceResponseDTOs = services.Select(s => new ServiceResponseDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        ShopId = s.ShopId
                    }).ToList();

                    return new BaseResponseDTO<List<ServiceResponseDTO>>
                    {
                        Flag = true,
                        Message = $"{services.Count} services found",
                        Data = serviceResponseDTOs
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = ex.Message
                    };
                }
            }
        }

        public async Task<BaseResponseDTO> GetServiceById(string id)
        {
            //check if id is null or empty
            if (string.IsNullOrEmpty(id))
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "Service Id is null or empty"
                };
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var service = await dbContext.Services.FirstOrDefaultAsync(s => s.Id == id);

                    //check if service exists
                    if (service == null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "Service not found"
                        };
                    }

                    return new BaseResponseDTO<Service>
                    {
                        Flag = true,
                        Message = "Service found",
                        Data = service
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = ex.Message
                    };
                }
            }
        }

        public async Task<BaseResponseDTO> GetServicesByShopId(string shopId)
        {
            //check if shopId is null or empty
            if (string.IsNullOrEmpty(shopId))
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "Shop Id is null or empty"
                };
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var services = await dbContext.Services.Where(s => s.ShopId == shopId).ToListAsync();

                    //check if services exist
                    if (services == null || services.Count == 0)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "No services found"
                        };
                    }

                    return new BaseResponseDTO<List<Service>>
                    {
                        Flag = true,
                        Message = $"{services.Count} services found",
                        Data = services
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = ex.Message
                    };
                }
            }
        }

        public async Task<BaseResponseDTO> Update(ServiceRequestDTO serviceRequestDTO)
        {
            //check if serviceRequestDTO is null
            if (serviceRequestDTO == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "ServiceRequestDTO is null"
                };
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var service = await dbContext.Services.FirstOrDefaultAsync(s => s.Id == serviceRequestDTO.Id);

                    //check if service exists
                    if (service == null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "Service not found"
                        };
                    }

                    service.Name = serviceRequestDTO.Name;
                    service.Price = serviceRequestDTO.Price;
                    service.ShopId = serviceRequestDTO.ShopId;

                    dbContext.Services.Update(service);
                    await dbContext.SaveChangesAsync();

                    return new BaseResponseDTO<Service>
                    {
                        Flag = true,
                        Message = "Service updated successfully",
                        Data = service
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = ex.Message
                    };
                }
            }
        }
    }
}
