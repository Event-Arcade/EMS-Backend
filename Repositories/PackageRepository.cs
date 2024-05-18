using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.Package;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Enums;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICloudProviderRepository _cloudProvider;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public PackageRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cloudProvider = cloudProvider;
            _configuration = configuration;
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
                    if (!await _userManager.IsInRoleAsync(user, "Client"))
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
                        CreatedAt = DateTime.Now
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
                        CreatedAt = package.CreatedAt,
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
                            CreatedAt = package.CreatedAt,
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
                        CreatedAt = package.CreatedAt,
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

                    var package = await dbContext.Packages.Where(p => p.UserId == userId && p.Id == id).Include(p => p.SubPackages).FirstOrDefaultAsync();
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
                        CreatedAt = package.CreatedAt,
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
    }
}
