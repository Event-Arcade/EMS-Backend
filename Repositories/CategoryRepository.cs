using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class CategoryRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, IConfiguration configuration) : ICategoryRepository
    {
        public async Task<BaseResponseDTO> AddCategory(BaseRequestDTO categoryRequestDTO)
        {
            //check if categoryRequestDTO is null
            if (categoryRequestDTO == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "CategoryRequestDTO is null"
                };
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //check if category exists
                var categoryExists = await dbContext.Categories.AnyAsync(c => c.Name == categoryRequestDTO.Name);
                if (categoryExists)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "Category already exists"
                    };
                }

                //upload category static data
                var (result , path) = await cloudProvider.UploadFile(categoryRequestDTO.Image, configuration["StorageDirectories:CategoryImages"]);
                if (!result)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = path
                    };
                }

                try
                {
                    var category = new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = categoryRequestDTO.Name,
                        Description = categoryRequestDTO.Description,
                        CategoryImage = path
                    };

                    //add category to database
                    await dbContext.Categories.AddAsync(category);
                    await dbContext.SaveChangesAsync();

                    //get new category
                    var newCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
                    //get image link
                    var imageUrl = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImage);
                    newCategory.CategoryImage = imageUrl;

                    return new BaseResponseDTO<Category>
                    {
                        Flag = true,
                        Message = "Category added successfully",
                        Data = newCategory
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

        public async Task<BaseResponseDTO> DeleteCategory(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "CategoryId is null or empty"
                };
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "Category not found"
                        };
                    }

                    //remove category image from cloud
                    var removeResult = await cloudProvider.RemoveFile(category.CategoryImage);
                    if (!removeResult)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "Failed to delete category image"
                        };
                    }

                    //delete category from database
                    dbContext.Categories.Remove(category);
                    await dbContext.SaveChangesAsync();

                    return new BaseResponseDTO
                    {
                        Flag = true,
                        Message = "Category deleted successfully"
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

        public async Task<BaseResponseDTO<List<Category>>> GetAllCategories()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var categories = await dbContext.Categories.ToListAsync();
                    if (categories == null)
                    {
                        return new BaseResponseDTO<List<Category>>()
                        {
                            Flag = false,
                            Message = "Categories not found",
                            Data = new List<Category>()
                        };
                    }

                    var categoryList = new List<Category>();
                    foreach (var category in categories)
                    {
                        //get image link
                        var imageUrl = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImage);
                        category.CategoryImage = imageUrl;
                        //add category to list
                        categoryList.Add(category);
                    }

                    return new BaseResponseDTO<List<Category>>()
                    {
                        Flag = true,
                        Message = "Categories found",
                        Data = categoryList
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO<List<Category>>()
                    {
                        Flag = false,
                        Message = ex.Message,
                        Data = new List<Category>()
                    };
                }
            }
        }

        public async Task<BaseResponseDTO<Category>> GetCategoryById(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return new BaseResponseDTO<Category>()
                {
                    Flag = false,
                    Message = "CategoryId is null or empty"
                };
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        return new BaseResponseDTO<Category>()
                        {
                            Flag = false,
                            Message = "Category not found"
                        };
                    }

                    //get image link
                    var imageUrl = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImage);
                    category.CategoryImage = imageUrl;

                    return new BaseResponseDTO<Category>
                    {
                        Flag = true,
                        Message = "Category found",
                        Data = category
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponseDTO<Category>()
                    {
                        Flag = false,
                        Message = ex.Message
                    };
                }
            }
        }

        public async Task<BaseResponseDTO> UpdateCategory(BaseRequestDTO categoryRequestDTO)
        {
            if (categoryRequestDTO == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "CategoryRequestDTO is null"
                };
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                //check if category exists
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryRequestDTO.Id);
                    if (category == null)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = "Category not found"
                        };
                    }

                    category.Name = categoryRequestDTO.Name;
                    category.Description = categoryRequestDTO.Description;

                    //store previous image path
                    var previousImagePath = category.CategoryImage;

                    //upload category static data
                    var (result, path) = await cloudProvider.UploadFile(categoryRequestDTO.Image, configuration["StorageDirectories:CategoryImages"]);
                    if (!result)
                    {
                        return new BaseResponseDTO
                        {
                            Flag = false,
                            Message = path
                        };
                    }
                    category.CategoryImage = path;

                    //remove previous image from cloud
                    await cloudProvider.RemoveFile(previousImagePath);

                    //update category
                    dbContext.Categories.Update(category);
                    await dbContext.SaveChangesAsync();

                    //get image link
                    var categoryImageUrl = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImage);
                    //return updated category
                    var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryRequestDTO.Id);

                    return new BaseResponseDTO<Category>
                    {
                        Flag = true,
                        Message = "Category updated successfully",
                        Data = updatedCategory
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
