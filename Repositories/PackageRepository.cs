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

        public PackageRepository(IServiceScopeFactory serviceScopeFactory, UserManager<ApplicationUser> userManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _userManager = userManager;
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
                        dbContext.SubPackages.Add(newSubPackage);
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

                    // delete subpackages
                    var subPackages = await dbContext.SubPackages.Where(sp => sp.PackageId == package.Id).ToListAsync();
                    foreach (var subPackage in subPackages)
                    {
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
        public async Task<BaseResponseDTO<IEnumerable<PackageResponseDTO>>> FindAllAsync()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var packages = await dbContext.Packages.ToListAsync();
                    var packageResponseList = new List<PackageResponseDTO>();
                    foreach (var package in packages)
                    {
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
                                OrderTime = subPackage.OrderTime,
                                Description = subPackage.Description,
                                ServiceId = subPackage.ServiceId,
                                Status = subPackage.Status
                            });
                        }

                        packageResponseList.Add(packageResponse);
                    }

                    return new BaseResponseDTO<IEnumerable<PackageResponseDTO>>
                    {
                        Data = packageResponseList,
                        Message = "Packages found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<PackageResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
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
    }
}
