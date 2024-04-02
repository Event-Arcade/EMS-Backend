using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class CategoryRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider) : ICategoryRepository
    {
        public async Task<CategoryResponse> AddCategory(CategoryRequestDTO categoryRequestDTO)
        {
            //check if categoryRequestDTO is null
            if (categoryRequestDTO == null)
            {
                return new CategoryResponse(false, "CategoryRequestDTO is null", null);
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //check if category exists
                var categoryExists = await dbContext.Categories.AnyAsync(c => c.Name == categoryRequestDTO.Name);
                if (categoryExists)
                {
                    return new CategoryResponse(false, "Category already exists", null);
                }

                //upload category static data
                var uploadResult = await cloudProvider.UploadFile(categoryRequestDTO.CategoryImage, "admin/category");
                if (!uploadResult.Item1)
                {
                    return new CategoryResponse(false, uploadResult.Item2, null);
                }


                try
                {
                    var category = new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = categoryRequestDTO.Name,
                        Description = categoryRequestDTO.Description,
                        CategoryImage = uploadResult.Item2
                    };

                    await dbContext.Categories.AddAsync(category);
                    await dbContext.SaveChangesAsync();
                    
                    //get image link
                    var imageUrl = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImage);

                    return new CategoryResponse(true, "Category created successfully", new CategoryResponseDTO
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        CategoryImage = imageUrl
                    });
                }
                catch (Exception ex)
                {
                    return new CategoryResponse(false, ex.Message, null);
                }
            }
        }

        public async Task<GeneralResponse> DeleteCategory(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return new GeneralResponse(false, "CategoryId is null or empty");
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        return new GeneralResponse(false, "Category not found");
                    }

                    //remove category image from cloud
                    var removeResult = await cloudProvider.RemoveFile(category.CategoryImage);
                    if (!removeResult)
                    {
                        return new GeneralResponse(false, "Failed to remove category image");
                    }

                    //delete category from database
                    dbContext.Categories.Remove(category);
                    await dbContext.SaveChangesAsync();

                    return new GeneralResponse(true, "Category deleted successfully");
                }
                catch (Exception ex)
                {
                    return new GeneralResponse(false, ex.Message);
                }
            }
        }

        public async Task<CategoryListResponse> GetAllCategories()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var categories = await dbContext.Categories.ToListAsync();
                    if (categories == null)
                    {
                        return new CategoryListResponse(false, "No categories found", null);
                    }

                    var categoryList = new List<CategoryResponseDTO>();
                    foreach (var category in categories)
                    {
                        //get image link
                        var imageUrl = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImage);

                        //add category to list
                        categoryList.Add(new CategoryResponseDTO
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Description = category.Description,
                            CategoryImage = imageUrl
                        });
                    }

                    return new CategoryListResponse(true, "Categories found", categoryList);
                }
                catch (Exception ex)
                {
                    return new CategoryListResponse(false, ex.Message, null);
                }
            }
        }

        public async Task<CategoryResponse> GetCategoryById(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return new CategoryResponse(false, "CategoryId is null or empty", null);
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        return new CategoryResponse(false, "Category not found", null);
                    }

                    //get image link
                    var imageUrl = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImage);

                    return new CategoryResponse(true, "Category found", new CategoryResponseDTO
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        CategoryImage = imageUrl
                    });
                }
                catch (Exception ex)
                {
                    return new CategoryResponse(false, ex.Message, null);
                }
            }
        }

        public async Task<CategoryResponse> UpdateCategory(CategoryRequestDTO categoryRequestDTO)
        {
            if (categoryRequestDTO == null)
            {
                return new CategoryResponse(false, "CategoryRequestDTO is null", null);
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
                        return new CategoryResponse(false, "Category not found", null);
                    }

                    category.Name = categoryRequestDTO.Name;
                    category.Description = categoryRequestDTO.Description;

                    //upload category static data
                    var uploadResult = await cloudProvider.UploadFile(categoryRequestDTO.CategoryImage, "admin/category");
                    if (!uploadResult.Item1)
                    {
                        return new CategoryResponse(false, uploadResult.Item2, null);
                    }
                    category.CategoryImage = uploadResult.Item2;

                    //update category
                    dbContext.Categories.Update(category);
                    await dbContext.SaveChangesAsync();

                    //get image link
                    var categoryImageUrl = cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImage);

                    return new CategoryResponse(true, "Category updated successfully", new CategoryResponseDTO
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        CategoryImage = categoryImageUrl
                    });
                }
                catch (Exception ex)
                {
                    return new CategoryResponse(false, ex.Message, null);
                }
            }
        }
        
    }
}
