using System.Linq.Expressions;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.Package;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.SubPackage;
using EMS.BACKEND.API.Enums;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationRepository _notificationRepository;

        public PackageRepository(IServiceScopeFactory serviceScopeFactory, INotificationRepository notificationRepository, UserManager<ApplicationUser> userManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponseDTO<PackageResponseDTO>> CreateAsync(string userId, PackageRequestDTO entity)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "User not found",
                            Flag = false
                        };
                    }

                    // check user is client
                    if (!await _userManager.IsInRoleAsync(user, "client"))
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "You cannot create package. You are not a client.",
                            Flag = false
                        };
                    }

                    // check subpackages count, if it is cancelled, then delete the package
                    if (entity.SubPackages.Count == 0)
                    {
                        throw new Exception("Subpackages not found");
                    }

                    var package = new Package
                    {
                        UserId = userId,
                        Status = PackageStatus.Pending,
                    };
                    dbContext.Packages.Add(package);
                    await dbContext.SaveChangesAsync();

                    // create subpackages
                    foreach (var subPackage in entity.SubPackages)
                    {
                        var newSubPackage = new SubPackage
                        {
                            PackageId = package.Id,
                            ServiceId = subPackage.ServiceId,
                            OrderTime = subPackage.OrderTime,
                            Description = subPackage.Description,
                            Status = PackageStatus.Pending
                        };
                       var subpackagetemp = dbContext.SubPackages.Add(newSubPackage);

                        // get the service
                        var service = await dbContext.ShopServices.Where(s => s.Id == subPackage.ServiceId).FirstOrDefaultAsync();
                        if (service == null)
                        {
                            return new BaseResponseDTO<PackageResponseDTO>
                            {
                                Message = "Service not found",
                                Flag = false
                            };
                        }

                        // get the shop
                        var shop = await dbContext.Shops.Where(s => s.Id == service.ShopId).FirstOrDefaultAsync();

                        if (shop != null && shop.OwnerId != null)
                        {
                            // send the notification to the vendor
                            await _notificationRepository.AddNotification("New Order", $"You have a new order from {user.FirstName}", DatabaseChangeEventType.Add, null, shop.OwnerId, EntityType.Package, subPackage.ServiceId, null);

                            // send the database change notification to the shop owner
                            await _notificationRepository.SendDatabaseChangeNotificationToUser(DatabaseChangeEventType.Add, EntityType.SubPackage, subpackagetemp.Entity.Id, shop.OwnerId);
                        }


                    }

                    await dbContext.SaveChangesAsync();

                    // create package response
                    var packageResponse = new PackageResponseDTO
                    {
                        Id = package.Id,
                        Status = package.Status,
                        UserId = package.UserId
                    };

                    // create subpackage response
                    packageResponse.SubPackages = new List<SubPackageResponseDTO>();
                    var subPackages = await dbContext.SubPackages.Where(sp => sp.PackageId == package.Id).ToListAsync();
                    foreach (var subPackage in subPackages)
                    {
                        packageResponse.SubPackages.Add(new SubPackageResponseDTO
                        {
                            Id = subPackage.Id,
                            PackageId = subPackage.PackageId,
                            ServiceId = subPackage.ServiceId,
                            OrderTime = subPackage.OrderTime,
                            Description = subPackage.Description,
                            Status = subPackage.Status
                        });
                    }

                    return new BaseResponseDTO<PackageResponseDTO>
                    {
                        Data = packageResponse,
                        Message = "Package created successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<PackageResponseDTO>
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
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return new BaseResponseDTO
                        {
                            Message = "User not found",
                            Flag = false
                        };
                    }

                    // check user is client
                    if (!await _userManager.IsInRoleAsync(user, "Client"))
                    {
                        return new BaseResponseDTO
                        {
                            Message = "You cannot delete package. You are not a client.",
                            Flag = false
                        };
                    }

                    var package = await dbContext.Packages.Where(p => p.UserId == userId && p.Id == id).FirstOrDefaultAsync();
                    if (package == null)
                    {
                        return new BaseResponseDTO
                        {
                            Message = "Package not found",
                            Flag = false
                        };
                    }

                    // delete subpackages if all subpackage status are not approved
                    var subPackages = await dbContext.SubPackages.Where(sp => sp.PackageId == package.Id).ToListAsync();

                    if (subPackages.Count == 0)
                    {
                        return new BaseResponseDTO
                        {
                            Message = "Package deleted successfully",
                            Flag = true
                        };
                    }

                    if (subPackages.Any(sp => sp.Status == PackageStatus.Approved))
                    {
                        return new BaseResponseDTO
                        {
                            Message = "You cannot delete the package. Some subpackages are approved.",
                            Flag = false
                        };
                    }

                    foreach (var subPackage in subPackages)
                    {
                        // send the notification to the vendor
                        var shpService = await dbContext.ShopServices.Where(s => s.Id == subPackage.ServiceId).FirstOrDefaultAsync();
                        if (shpService != null)
                        {
                            var shop = await dbContext.Shops.Where(s => s.Id == shpService.ShopId).FirstOrDefaultAsync();
                            if (shop != null && shop.OwnerId != null)
                            {
                                await _notificationRepository.AddNotification("Order Cancelled", "Your order is cancelled", DatabaseChangeEventType.Delete, null, shop.OwnerId, EntityType.SubPackage, subPackage.Id, null);

                                // send the database change notification to the shop owner
                                await _notificationRepository.SendDatabaseChangeNotificationToUser(DatabaseChangeEventType.Delete, EntityType.SubPackage, subPackage.Id, shop.OwnerId);
                            }
                        }
                        dbContext.SubPackages.Remove(subPackage);
                    }

                    dbContext.Packages.Remove(package);
                    await dbContext.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Message = "Package deleted successfully",
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
        public Task<BaseResponseDTO<IEnumerable<PackageResponseDTO>>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponseDTO<PackageResponseDTO>> FindByIdAsync(int id)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var package = await dbContext.Packages.Where(p => p.Id == id).Include(p => p.SubPackages).FirstOrDefaultAsync();
                    if (package == null)
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "Package not found",
                            Flag = false
                        };
                    }

                    var packageResponse = new PackageResponseDTO
                    {
                        Id = package.Id,
                        Status = package.Status,
                        UserId = package.UserId
                    };

                    // create subpackage response
                    packageResponse.SubPackages = new List<SubPackageResponseDTO>();
                    var subPackages = await dbContext.SubPackages.Where(sp => sp.PackageId == package.Id).ToListAsync();
                    foreach (var subPackage in subPackages)
                    {
                        packageResponse.SubPackages.Add(new SubPackageResponseDTO
                        {
                            Id = subPackage.Id,
                            PackageId = subPackage.PackageId,
                            ServiceId = subPackage.ServiceId,
                            OrderTime = subPackage.OrderTime,
                            Description = subPackage.Description,
                            Status = subPackage.Status
                        });
                    }

                    return new BaseResponseDTO<PackageResponseDTO>
                    {
                        Data = packageResponse,
                        Message = "Package found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<PackageResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };

            }
        }

        public async Task<BaseResponseDTO<ICollection<SubPackageResponseDTO>>> GetSubPackages(string userId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return new BaseResponseDTO<ICollection<SubPackageResponseDTO>>
                        {
                            Message = "User not found",
                            Flag = false
                        };
                    }

                    // check user is vendor
                    if (!await _userManager.IsInRoleAsync(user, "vendor"))
                    {
                        return new BaseResponseDTO<ICollection<SubPackageResponseDTO>>
                        {
                            Message = "You cannot get subpackages. You are not a vendor.",
                            Flag = false
                        };
                    }

                    var myShop = await dbContext.Shops.Where(s => s.OwnerId == userId).FirstOrDefaultAsync();
                    if (myShop == null)
                    {
                        return new BaseResponseDTO<ICollection<SubPackageResponseDTO>>
                        {
                            Message = "Shop not found",
                            Flag = false
                        };
                    }

                    var myServices = await dbContext.ShopServices.Where(s => s.ShopId == myShop.Id).ToListAsync();

                    var mySubPackages = new List<SubPackage>();
                    var allSubPackages = await dbContext.SubPackages.ToListAsync();
                    foreach (var subPackage in allSubPackages)
                    {
                        foreach (var service in myServices)
                        {
                            if (subPackage.ServiceId == service.Id)
                            {
                                mySubPackages.Add(subPackage);
                            }
                        }
                    }

                    if (mySubPackages.Count == 0)
                    {
                        return new BaseResponseDTO<ICollection<SubPackageResponseDTO>>
                        {
                            Message = "Subpackages not found",
                            Flag = false
                        };
                    }

                    var subPackageResponseList = new List<SubPackageResponseDTO>();
                    foreach (var subPackage in mySubPackages)
                    {
                        subPackageResponseList.Add(new SubPackageResponseDTO
                        {
                            Id = subPackage.Id,
                            PackageId = subPackage.PackageId,
                            ServiceId = subPackage.ServiceId,
                            OrderTime = subPackage.OrderTime,
                            Description = subPackage.Description,
                            Status = subPackage.Status
                        });
                    }

                    return new BaseResponseDTO<ICollection<SubPackageResponseDTO>>
                    {
                        Data = subPackageResponseList,
                        Message = "Subpackages found",
                        Flag = true
                    };

                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ICollection<SubPackageResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<PackageResponseDTO>> UpdateAsync(string userId, int id, PackageRequestDTO entity)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "User not found",
                            Flag = false
                        };
                    }

                    // check user is client
                    if (!await _userManager.IsInRoleAsync(user, "Client"))
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "You cannot update package. You are not a client.",
                            Flag = false
                        };
                    }

                    var package = await dbContext.Packages.Where(p => p.UserId == userId && p.Id == id).FirstOrDefaultAsync();
                    if (package == null)
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "Package not found",
                            Flag = false
                        };
                    }

                    // update package
                    dbContext.Packages.Update(package);
                    await dbContext.SaveChangesAsync();

                    // create package response
                    var packageResponse = new PackageResponseDTO
                    {
                        Id = package.Id,
                        Status = package.Status,
                        UserId = package.UserId
                    };

                    // create subpackage response
                    packageResponse.SubPackages = new List<SubPackageResponseDTO>();
                    foreach (var subPackage in package.SubPackages)
                    {
                        packageResponse.SubPackages.Add(new SubPackageResponseDTO
                        {
                            Id = subPackage.Id,
                            PackageId = subPackage.PackageId,
                            ServiceId = subPackage.ServiceId,
                            OrderTime = subPackage.OrderTime,
                            Description = subPackage.Description,
                            Status = subPackage.Status
                        });
                    }

                    return new BaseResponseDTO<PackageResponseDTO>
                    {
                        Data = packageResponse,
                        Message = "Package updated successfully",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<PackageResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<PackageResponseDTO>> UpdateSubPackage(string userId, int id, SubPackageRequestDTO subPackageRequest)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "User not found",
                            Flag = false
                        };
                    }

                    // check user is client
                    if (!await _userManager.IsInRoleAsync(user, "vendor"))
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "You cannot update subpackage. You are not a vendor.",
                            Flag = false
                        };
                    }

                    // find the subpackage
                    var subPackage = await dbContext.SubPackages.Where(sp => sp.Id == subPackageRequest.Id).FirstOrDefaultAsync();
                    if (subPackage == null)
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "Subpackage not found",
                            Flag = false
                        };
                    }

                    // update subpackage
                    subPackage.Status = subPackageRequest.Status;
                    dbContext.SubPackages.Update(subPackage);
                    await dbContext.SaveChangesAsync();

                    var package = await dbContext.Packages.Where(p => p.Id == subPackage.PackageId).Include(p => p.SubPackages).FirstOrDefaultAsync();
                    if (package == null)
                    {
                        return new BaseResponseDTO<PackageResponseDTO>
                        {
                            Message = "Package not found",
                            Flag = false
                        };
                    }


                    var response = new PackageResponseDTO()
                    {
                        Id = package.Id,
                        Status = package.Status,
                        UserId = package.UserId
                    };

                    response.SubPackages = new List<SubPackageResponseDTO>();
                    foreach (var subPkage in package.SubPackages)
                    {
                        response.SubPackages.Add(new SubPackageResponseDTO
                        {
                            Id = subPkage.Id,
                            PackageId = subPkage.PackageId,
                            ServiceId = subPkage.ServiceId,
                            OrderTime = subPkage.OrderTime,
                            Description = subPkage.Description,
                            Status = subPkage.Status
                        });
                    }

                    // notify the client
                    await _notificationRepository.AddNotification($"Order Status {package.Id}", $"Your order status is updated to {subPackage.Status}", DatabaseChangeEventType.Update, null, package.UserId, EntityType.SubPackage, response.Id, null);

                    // send the database change notification to the client
                    await _notificationRepository.SendDatabaseChangeNotificationToUser(DatabaseChangeEventType.Update, EntityType.Package, response.Id, package.UserId);

                    return new BaseResponseDTO<PackageResponseDTO>
                    {
                        Data = response,
                        Message = "Subpackage updated successfully",
                        Flag = true
                    };

                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<PackageResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<SubPackageResponseDTO>> GetSubPackageById(string userId, int id)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return new BaseResponseDTO<SubPackageResponseDTO>
                        {
                            Message = "User not found",
                            Flag = false
                        };
                    }

                    // check user is vendor
                    if (!await _userManager.IsInRoleAsync(user, "vendor"))
                    {
                        return new BaseResponseDTO<SubPackageResponseDTO>
                        {
                            Message = "You cannot get subpackage. You are not a vendor.",
                            Flag = false
                        };
                    }

                    var subPackage = await dbContext.SubPackages.Where(sp => sp.Id == id).FirstOrDefaultAsync();
                    if (subPackage == null)
                    {
                        return new BaseResponseDTO<SubPackageResponseDTO>
                        {
                            Message = "Subpackage not found",
                            Flag = false
                        };
                    }

                    return new BaseResponseDTO<SubPackageResponseDTO>
                    {
                        Data = new SubPackageResponseDTO
                        {
                            Id = subPackage.Id,
                            PackageId = subPackage.PackageId,
                            ServiceId = subPackage.ServiceId,
                            OrderTime = subPackage.OrderTime,
                            Description = subPackage.Description,
                            Status = subPackage.Status
                        },
                        Message = "Subpackage found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<SubPackageResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

        public async Task<BaseResponseDTO<ICollection<PackageResponseDTO>>> GetAllPackages(string userId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        throw new Exception("User not found");
                    }

                    // check user is client
                    if (!await _userManager.IsInRoleAsync(user, "client"))
                    {
                        throw new Exception("You cannot get packages. You are not a client.");
                    }

                    var packages = await dbContext.Packages.Where(p => p.UserId == userId).Include(p => p.SubPackages).ToListAsync();
                    if (packages.Count == 0)
                    {
                        throw new Exception("Packages not found");
                    }

                    var responseList = new List<PackageResponseDTO>();
                    foreach (var package in packages)
                    {
                        var response = new PackageResponseDTO
                        {
                            Id = package.Id,
                            Status = package.Status,
                            UserId = package.UserId
                        };

                        response.SubPackages = new List<SubPackageResponseDTO>();
                        foreach (var subPackage in package.SubPackages)
                        {
                            response.SubPackages.Add(new SubPackageResponseDTO
                            {
                                Id = subPackage.Id,
                                PackageId = subPackage.PackageId,
                                ServiceId = subPackage.ServiceId,
                                OrderTime = subPackage.OrderTime,
                                Description = subPackage.Description,
                                Status = subPackage.Status
                            });
                        }

                        responseList.Add(response);
                    }

                    return new BaseResponseDTO<ICollection<PackageResponseDTO>>
                    {
                        Data = responseList,
                        Message = "Packages found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<ICollection<PackageResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
    }
}
