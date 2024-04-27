using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class CategoryRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider,
                                        IConfiguration configuration, IUserAccountRepository accountRepository) : ICategoryRepository
    {
        public async Task<BaseResponseDTO<String>> CreateAsync(Category entity)
        {
            // Check entity is null
            if (entity == null)
            {
                throw new Exception("Request is null");
            }

            //TODO: Check if the user is admin and validated by the token
            using (var scope = serviceScopeFactory.CreateScope())
            {

                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    //Get current user
                    var user = await accountRepository.GetMe();
                    // Check if category already exists
                    var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == entity.Name);
                    if (category != null)
                    {
                        throw new Exception("Category already exists");
                    }

                    // Upload category image to S3
                    var (flag, filePath) = await cloudProvider.UploadFile(entity.CategoryImage, configuration["StorageDirectories:CategoryImages"]);

                    if (!flag)
                    {
                        throw new Exception("Failed to upload category image");
                    }

                    // Create new category
                    var newCategory = new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = entity.Name,
                        Description = entity.Description,
                        CategoryImagePath = filePath,
                        UserId = user.Data.Id
                    };
                    // Save category
                    await context.Categories.AddAsync(newCategory);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO<String>
                    {
                        Message = "Category created successfully",
                        Flag = true
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO<String>
                    {
                        Message = ex.Message,
                        Flag = false
                    };
                }
            }

        }
        public async Task<BaseResponseDTO<String>> DeleteAsync(string id)
        {
            // Check id is null
            if (id == null)
            {
                throw new Exception("Requested id is null");
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    // Find category by id
                    var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                    if (category == null)
                    {
                        throw new Exception("Category not found");
                    }

                    // Remove category image from S3
                    if(category.CategoryImagePath != "images/category-images/default.png")
                    {
                        await cloudProvider.RemoveFile(category.CategoryImagePath);
                    }

                    // Delete category
                    context.Categories.Remove(category);
                    await context.SaveChangesAsync();

                    return new BaseResponseDTO<String>
                    {
                        Message = "Category deleted successfully",
                        Flag = true
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO<String>
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

                    // Assign pre signed URL to each category
                    foreach (var category in categories)
                    {
                        category.CategoryImagePath = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImagePath);
                    }

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
                        throw new Exception("Category not found");
                    }

                    // Asiign pre signed URL to category
                    category.CategoryImagePath = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImagePath);

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
        public async Task<BaseResponseDTO> UpdateAsync(String id,Category entity)
        {
            // Check entity is null
            if (entity == null)
            {
                throw new Exception("Reqest is null");
            }

            // Update category
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    // Find category by name
                    var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                    if (category == null)
                    {
                        throw new Exception("Category not found");
                    }

                    // Create new category
                    if (entity.Name != null)
                    {
                        category.Name = entity.Name;
                    }
                    if (entity.Description != null)
                    {
                        category.Description = entity.Description;
                    }
                    if (entity.CategoryImage != null)
                    {
                        //Remove the old category image and upload the new one
                        var (flag, filePath) = await cloudProvider.UpdateFile(entity.CategoryImage, configuration["StorageDirectories:CategoryImages"], category.CategoryImagePath);
                        if (!flag)
                        {
                            throw new Exception("Failed to update category image");
                        }
                        category.CategoryImagePath = filePath;
                    }

                    // Update category
                    context.Categories.Update(category);
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
