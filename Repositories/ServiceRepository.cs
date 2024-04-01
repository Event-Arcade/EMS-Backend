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
        public async Task<ServiceResponse> Create(ServiceRequestDTO serviceRequestDTO)
        {
            if (serviceRequestDTO == null)
            {
                return new ServiceResponse(false, "ServiceRequestDTO is null", null);
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
                        return new ServiceResponse(false, "Service already exists", null);
                    }

                    await dbContext.Services.AddAsync(service);
                    await dbContext.SaveChangesAsync();

                    return new ServiceResponse(true, "Service created successfully", new ServiceResponseDTO
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Price = service.Price,
                        ShopId = service.ShopId
                    });
                }
                catch (Exception ex)
                {
                    return new ServiceResponse(false, ex.Message, null);
                }
            }
        }

        public async Task<ServiceResponse> Delete(string id)
        {
            //implement delete service
            if (string.IsNullOrEmpty(id))
            {
                return new ServiceResponse(false, "Service Id is null or empty", null);
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var service = await dbContext.Services.FirstOrDefaultAsync(s => s.Id == id);
                    if (service == null)
                    {
                        return new ServiceResponse(false, "Service not found", null);
                    }

                    dbContext.Services.Remove(service);
                    await dbContext.SaveChangesAsync();

                    return new ServiceResponse(true, "Service deleted successfully", null);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse(false, ex.Message, null);
                }
            }
        }

        public async Task<ServiceListResponse> GetAllServices()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var services = dbContext.Services.ToList();
                    if (services == null)
                    {
                        return new ServiceListResponse(false, "No services found", null);
                    }

                    var serviceResponseDTOs = services.Select(s => new ServiceResponseDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        ShopId = s.ShopId
                    }).ToList();

                    return new ServiceListResponse(true, $"{services.Count} services found", serviceResponseDTOs);
                }
                catch (Exception ex)
                {
                    return new ServiceListResponse(false, ex.Message, null);
                }
            }
        }

        public async Task<ServiceResponse> GetServiceById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ServiceResponse(false, "Service Id is null or empty", null);
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var service = await dbContext.Services.FirstOrDefaultAsync(s => s.Id == id);
                    if (service == null)
                    {
                        return new ServiceResponse(false, "Service not found", null);
                    }

                    return new ServiceResponse(true, "Service found", new ServiceResponseDTO
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Price = service.Price,
                        ShopId = service.ShopId
                    });
                }
                catch (Exception ex)
                {
                    return new ServiceResponse(false, ex.Message, null);
                }
            }
        }

        public async Task<ServiceListResponse> GetServicesByShopId(string shopId)
        {
            if (string.IsNullOrEmpty(shopId))
            {
                return new ServiceListResponse(false, "Shop Id is null or empty", null);
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var services = await dbContext.Services.Where(s => s.ShopId == shopId).ToListAsync();
                    if (services == null)
                    {
                        return new ServiceListResponse(false, "No services found", null);
                    }

                    var serviceResponseDTOs = services.Select(s => new ServiceResponseDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        ShopId = s.ShopId
                    }).ToList();

                    return new ServiceListResponse(true, $"{services.Count} services found", serviceResponseDTOs);
                }
                catch (Exception ex)
                {
                    return new ServiceListResponse(false, ex.Message, null);
                }
            }
        }

        public async Task<ServiceResponse> Update(ServiceRequestDTO serviceRequestDTO)
        {
            if (serviceRequestDTO == null)
            {
                return new ServiceResponse(false, "ServiceRequestDTO is null", null);
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var service = await dbContext.Services.FirstOrDefaultAsync(s => s.Id == serviceRequestDTO.Id);
                    if (service == null)
                    {
                        return new ServiceResponse(false, "Service not found", null);
                    }

                    service.Name = serviceRequestDTO.Name;
                    service.Price = serviceRequestDTO.Price;
                    service.ShopId = serviceRequestDTO.ShopId;

                    dbContext.Services.Update(service);
                    await dbContext.SaveChangesAsync();

                    return new ServiceResponse(true, "Service updated successfully", new ServiceResponseDTO
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Price = service.Price,
                        ShopId = service.ShopId
                    });
                }
                catch (Exception ex)
                {
                    return new ServiceResponse(false, ex.Message, null);
                }
            }
        }

        
    }
}
