using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Repositories
{
    public class PackageRepository(IServiceScopeFactory scopeFactory,
                                ISubPackageRepository subPackageRepository) : IPackageRepository
    {
        public async Task<BaseResponseDTO<String>> CreateAsync(Package entity)
        {
            try
            {
                //check if entity is null
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                using (var scope = scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var package = new Package
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedAt = entity.CreatedAt,
                        UserId = entity.UserId,
                    };

                    //create subpackages
                    foreach (var subPackage in entity.SubPackages)
                    {
                        var subPackageResponse = await subPackageRepository.CreateAsync(subPackage);
                        if (!subPackageResponse.Flag)
                        {
                            return new BaseResponseDTO<String>
                            {
                                Flag = false,
                                Message = "An error occured while creating subpackage"
                            };
                        }
                    }

                    //add package to db
                    await context.Packages.AddAsync(package);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO<String>
                    {
                        Flag = true,
                        Message = "Package created successfully"
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
        public async Task<BaseResponseDTO<String>> DeleteAsync(string id)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var package = await context.Packages.FindAsync(id);
                    if (package == null)
                    {
                        return new BaseResponseDTO<String>
                        {
                            Flag = false,
                            Message = "Package not found"
                        };
                    }
                    //delete subpackages
                    var response = await subPackageRepository.GetSubpackagesByUser(package.UserId);
                    foreach (var subPackage in response.Data)
                    {
                        var subPackageResponse = await subPackageRepository.DeleteAsync(subPackage.Id);
                        if (!subPackageResponse.Flag)
                        {
                            return new BaseResponseDTO<String>
                            {
                                Flag = false,
                                Message = "An error occured while deleting subpackage"
                            };
                        }
                    }

                    //delete package
                    context.Packages.Remove(package);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO<String>
                    {
                        Flag = true,
                        Message = "Package deleted successfully"
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

        //TODO : implement if want from the front end
        public Task<BaseResponseDTO<IEnumerable<Package>>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponseDTO<Package>> FindByIdAsync(string id)
        {
            try
            {
                //check if the entity is null
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                using (var scope = scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var package = await context.Packages.FindAsync(id);

                    if (package == null)
                    {
                        return new BaseResponseDTO<Package>
                        {
                            Flag = false,
                            Message = "Package not found!"
                        };
                    }

                    //get subpackages
                    var response = await subPackageRepository.GetSubpackagesByUser(package.UserId);
                    if (!response.Flag)
                    {
                        return new BaseResponseDTO<Package>
                        {
                            Flag = false,
                            Message = "An error occured while fetching subpackages"
                        };
                    }

                    //add subpackages to package
                    foreach (var subPackage in response.Data)
                    {
                        package.SubPackages.Add(subPackage);
                    }


                    return new BaseResponseDTO<Package>
                    {
                        Flag = true,
                        Data = package
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<Package>
                {
                    Flag = false,
                    Message = ex.Message
                };

            }
        }

        public async Task<BaseResponseDTO<IEnumerable<Package>>> GetAllPackagesByUser(string userId)
        {
            try
            {
                //check if the userId is null
                if (userId == null)
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                using (var scope = scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var packages = context.Packages.Where(x => x.UserId == userId).ToList();

                    //get the subpackages
                    foreach (var package in packages)
                    {
                        var response = await subPackageRepository.GetSubpackagesByUser(package.UserId);
                        if (!response.Flag)
                        {
                            return new BaseResponseDTO<IEnumerable<Package>>
                            {
                                Flag = false,
                                Message = "An error occured while fetching subpackages"
                            };
                        }

                        //add subpackages to package
                        foreach (var subPackage in response.Data)
                        {
                            package.SubPackages.Add(subPackage);
                        }
                    }
                    //check pacakges are null or empty
                    if (packages == null || packages.Count == 0)
                    {
                        return new BaseResponseDTO<IEnumerable<Package>>
                        {
                            Flag = false,
                            Message = "Packages not found!"
                        };
                    }

                    return new BaseResponseDTO<IEnumerable<Package>>
                    {
                        Flag = true,
                        Data = packages
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<Package>>
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<BaseResponseDTO> UpdateAsync(String id, Package entity)
        {
            try
            {
                //check if the entity is null
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                using (var scope = scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var package = await context.Packages.FindAsync(entity.Id);

                    if (package == null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "Package not found!"
                        };
                    }

                    //update package
                    package.CreatedAt = entity.CreatedAt;
                    package.UserId = entity.UserId;

                    //update subpackages
                    var response = await subPackageRepository.GetSubpackagesByUser(package.UserId);
                    if (!response.Flag)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "An error occured while fetching subpackages"
                        };
                    }

                    foreach (var subPackage in response.Data)
                    {
                        var subPackageResponse = await subPackageRepository.UpdateAsync(subPackage.Id, subPackage);
                        if (!subPackageResponse.Flag)
                        {
                            return new BaseResponseDTO
                            {
                                Flag = false,
                                Message = "An error occured while updating subpackage"
                            };
                        }
                    }

                    //update package
                    context.Packages.Update(package);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Flag = true,
                        Message = "Package updated successfully"
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
