using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class CategoryRepository(IServiceScopeFactory serviceScopeFactory) : ICategoryRepository
    {
        public async Task<CategoryResponse> AddCategory(CategoryRequestDTO categoryRequestDTO)
        {
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

                try
                {
                    var category = new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = categoryRequestDTO.Name
                    };

                    await dbContext.Categories.AddAsync(category);
                    await dbContext.SaveChangesAsync();

                    return new CategoryResponse(true, "Category created successfully", new CategoryResponseDTO
                    {
                        Id = category.Id,
                        Name = category.Name
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
                        categoryList.Add(new CategoryResponseDTO
                        {
                            Id = category.Id,
                            Name = category.Name
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

                    return new CategoryResponse(true, "Category found", new CategoryResponseDTO
                    {
                        Id = category.Id,
                        Name = category.Name
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
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryRequestDTO.Id);
                    if (category == null)
                    {
                        return new CategoryResponse(false, "Category not found", null);
                    }

                    category.Name = categoryRequestDTO.Name;
                    dbContext.Categories.Update(category);
                    await dbContext.SaveChangesAsync();

                    return new CategoryResponse(true, "Category updated successfully", new CategoryResponseDTO
                    {
                        Id = category.Id,
                        Name = category.Name
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
