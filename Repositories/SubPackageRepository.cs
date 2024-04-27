
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class SubPackageRepository(IServiceScopeFactory serviceScope) : ISubPackageRepository
    {
        public async Task<BaseResponseDTO<String>> CreateAsync(SubPackage entity)
        {
            try
            {
                //check if the entity is null
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                //add the entity to the database
                using (var scope = serviceScope.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    //set entity status to Pending
                    entity.Id = Guid.NewGuid().ToString();
                    entity.Status = Enums.PackageStatus.Pending;

                    await context.SubPackages.AddAsync(entity);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO<String>
                    {
                        Flag = true,
                        Message = "SubPackage created successfully!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<String> { Flag = false, Message = ex.Message };
            }
        }

        public async Task<BaseResponseDTO<String>> DeleteAsync(string id)
        {
            try
            {
                //check if the entity is null
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                using (var scope = serviceScope.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var subPackage = await context.SubPackages.FindAsync(id);

                    if (subPackage == null)
                    {
                        return new BaseResponseDTO<String>
                        {
                            Flag = false,
                            Message = "SubPackage not found!"
                        };
                    }

                    context.SubPackages.Remove(subPackage);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO<String>
                    {
                        Flag = true,
                        Message = "SubPackage deleted successfully!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<String>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponseDTO<IEnumerable<SubPackage>>> FindAllAsync()
        {
            try
            {
                //get all subpackages from the database
                using (var scope = serviceScope.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var subPackages = await context.SubPackages.ToListAsync();

                    return new BaseResponseDTO<IEnumerable<SubPackage>>
                    {
                        Flag = true,
                        Message = "SubPackages retrieved successfully!",
                        Data = subPackages
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<SubPackage>>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponseDTO<SubPackage>> FindByIdAsync(string id)
        {
            try
            {
                //check if the entity is null
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                using (var scope = serviceScope.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var subPackage = await context.SubPackages.FindAsync(id);

                    if (subPackage == null)
                    {
                        return new BaseResponseDTO<SubPackage>
                        {
                            Flag = false,
                            Message = "SubPackage not found!"
                        };
                    }

                    return new BaseResponseDTO<SubPackage>
                    {
                        Flag = true,
                        Message = "SubPackage retrieved successfully!",
                        Data = subPackage
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<SubPackage>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }


        public async Task<BaseResponseDTO<IEnumerable<SubPackage>>> GetSubpackagesByUser(string userId)
        {
            try
            {
                using (var scope = serviceScope.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    //get the packages of the user
                    var packages = await context.Packages.Where(p => p.User.Id == userId).ToListAsync();
                    //get the subpackages of the packages
                    var subPackages = new List<SubPackage>();
                    foreach (var package in packages)
                    {
                        var subPackagesInPackage = await context.SubPackages.Where(s => s.PackageId == package.Id).ToListAsync();
                        subPackages.AddRange(subPackagesInPackage);
                    }

                    return new BaseResponseDTO<IEnumerable<SubPackage>>
                    {
                        Flag = true,
                        Message = "SubPackages retrieved successfully!",
                        Data = subPackages
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<SubPackage>>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponseDTO> UpdateAsync(String id, SubPackage entity)
        {
            try
            {
                //check if the entity is null
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                using (var scope = serviceScope.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var subPackage = await context.SubPackages.FindAsync(entity.Id);

                    if (subPackage == null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "SubPackage not found!"
                        };
                    }

                    subPackage.Description = entity.Description;
                    subPackage.PackageId = entity.PackageId;
                    subPackage.ServiceId = entity.ServiceId;

                    context.SubPackages.Update(subPackage);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Flag = true,
                        Message = "SubPackage updated successfully!"
                    };
                }
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