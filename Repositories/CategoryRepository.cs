using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class CategoryRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider,
                                        IConfiguration configuration) : ICategoryRepository
    {
        public async Task<BaseResponseDTO> CreateAsync(Category entity)
        {
            // Check entity is null
            if (entity == null)
            {
                throw new Exception("Request is null");
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    // Check if category already exists
                    var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == entity.Name);
                    if (category != null)
                    {
                        throw new Exception("Category already exists");
                    }

                    // Upload category image to S3
                    var (flag, filePath) = await cloudProvider.UploadFile(entity.CategoryImage, configuration["StorageDirectories:CategoryImages"]);

                    if (flag)
                    {
                        entity.CategoryImagePath = filePath;
                    }
                    else
                    {
                        throw new Exception("Failed to upload category image");
                    }
                    // Save category
                    await context.Categories.AddAsync(entity);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Message = "Category created successfully",
                        Flag = true
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO<Category>
                    {
                        Message = ex.Message,
                        Flag = false
                    };
                }
            }

        }
        public async Task<BaseResponseDTO> DeleteAsync(string id)
        {
            // Check id is null
            if (id == null)
            {
                throw new Exception("Requested id is null");
            }

            // Find category by id
            var category = await FindByIdAsync(id);
            if (category == null)
            {
               throw new Exception("Category not found");
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    //Check whether any service is using the category
                    if (context.Services.Any(x => x.CategoryId == id))
                    {
                        throw new Exception("Category is in use");
                    }

                    // Remove category image from S3
                    var flag = await cloudProvider.RemoveFile(category.Data.CategoryImagePath);

                    if (!flag)
                    {
                        throw new Exception("Failed to delete category image");
                    }

                    // Delete category
                    context.Categories.Remove(category.Data);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Message = "Category deleted successfully",
                        Flag = true
                    };
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
        }
        public async Task<BaseResponseDTO<IEnumerable<Category>>> FindAllAsync()
        {
            try
            {
                // Find all categories
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var categories = await context.Categories.ToListAsync();


                    return new BaseResponseDTO<IEnumerable<Category>>
                    {
                        Data = categories,
                        Message = "Categories found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<Category>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<Category>> FindByIdAsync(string id)
        {
            // Check id is null
            if (id == null)
            {
                throw new Exception("Requested id is null");
            }

            try
            {
                // Find category by id
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                    if (category == null)
                    {
                        throw  new Exception("Category not found");
                    }

                    return new BaseResponseDTO<Category>
                    {
                        Data = category,
                        Message = "Category found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<Category>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO> UpdateAsync(Category entity)
        {
            // Check entity is null
            if (entity == null)
            {
                throw new Exception("Reqest is null");
            }

            // Find category by id
            var responseDTO =await FindByIdAsync(entity.Id);
            var category = responseDTO.Data;
            if (category == null)
            {
                throw new Exception("Category not found");
            }

            // Update category
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    //Update the category image
                    var (flag, newFilePath) = await cloudProvider.UpdateFile(entity.CategoryImage, configuration["StorageDirectories:CategoryImages"], category.CategoryImagePath);
                    if (flag)
                    {
                        entity.CategoryImagePath = newFilePath;
                    }
                    else
                    {
                        throw new Exception("Failed to update category image");
                    }

                    // Update category
                    context.Categories.Update(entity);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Message = "Category updated successfully",
                        Flag = true
                    };
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
        }
    }
}
