using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.Category;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Enums;
using EMS.BACKEND.API.Mappers;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICloudProviderRepository _cloudProvider;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationRepository _notificationRepository;

        public CategoryRepository(IServiceScopeFactory serviceScopeFactory, ICloudProviderRepository cloudProvider, IConfiguration configuration, UserManager<ApplicationUser> userManager, INotificationRepository notificationRepository)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cloudProvider = cloudProvider;
            _configuration = configuration;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponseDTO<CategoryResponseDTO>> CreateAsync(string userId, CategoryRequestDTO entity)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //Get the user and check if the user is admin
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        throw new Exception("User not found");
                    }

                    if (!await _userManager.IsInRoleAsync(user, "admin"))
                    {
                        throw new Exception("User is not an admin");
                    }

                    // Check if category already exists
                    var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == entity.Name);
                    if (category != null)
                    {
                        throw new Exception("Category already exists");
                    }

                    // Upload category image to S3
                    var (flag, filePath) = await _cloudProvider.UploadFile(entity.CategoryImage, _configuration["StorageDirectories:CategoryImages"]);

                    if (!flag)
                    {
                        throw new Exception("Failed to upload category image");
                    }

                    // Create new category
                    var newCategory = entity.ToCategory(filePath, user);
                    // Save category
                    await context.Categories.AddAsync(newCategory);
                    await context.SaveChangesAsync();

                    // Assign pre signed URL to category
                    var url = _cloudProvider.GeneratePreSignedUrlForDownload(newCategory.CategoryImagePath);

                    // send notifications to admins and vendors
                    await _notificationRepository.AddNotification("New Category", "A new category has been added",DatabaseChangeEventType.Add, "admin", null, EntityType.Category, newCategory.Id, userId);
                    await _notificationRepository.AddNotification("New Category", "A new category has been added",DatabaseChangeEventType.Add, "vendor", null, EntityType.Category, newCategory.Id, null);

                    // send database change event to all clients, vendors and admins
                    await _notificationRepository.SendDatabaseChangeNotification( DatabaseChangeEventType.Add, EntityType.Category, newCategory.Id, userId);

                    return new BaseResponseDTO<CategoryResponseDTO>
                    {
                        Message = "Category created successfully",
                        Flag = true,
                        Data = newCategory.ToCategoryResponseDTO(url)
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<CategoryResponseDTO>
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
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //Get the user and check if the user is admin
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        throw new Exception("User not found");
                    }

                    if (!await _userManager.IsInRoleAsync(user, "admin"))
                    {
                        throw new Exception("User is not an admin");
                    }

                    // Find category by id
                    var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                    if (category == null)
                    {
                        throw new Exception("Category not found");
                    }

                    // Remove category image from S3
                    if (category.CategoryImagePath != "images/category-images/default.png")
                    {
                        await _cloudProvider.RemoveFile(category.CategoryImagePath);
                    }

                    // Delete category
                    context.Categories.Remove(category);
                    await context.SaveChangesAsync();

                    // send notifications to admins 
                    await _notificationRepository.AddNotification("Category Deleted", "A category has been deleted",DatabaseChangeEventType.Delete, "admin", null, EntityType.Category, category.Id, userId);

                    // send database change event to all clients, vendors and admins
                    await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Delete, EntityType.Category, category.Id, userId);

                    return new BaseResponseDTO
                    {
                        Message = "Category deleted successfully",
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
        public async Task<BaseResponseDTO<IEnumerable<CategoryResponseDTO>>> FindAllAsync()
        {
            try
            {
                // Find all categories
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var categories = await context.Categories.ToListAsync();

                    // Assign pre signed URL to each category
                    foreach (var category in categories)
                    {
                        category.CategoryImagePath = _cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImagePath);
                    }

                    var categoryDtos = categories.Select(x => x.ToCategoryResponseDTO(x.CategoryImagePath));

                    return new BaseResponseDTO<IEnumerable<CategoryResponseDTO>>
                    {
                        Data = categoryDtos,
                        Message = "Categories found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<CategoryResponseDTO>>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<CategoryResponseDTO>> FindByIdAsync(int id)
        {
            try
            {
                // Find category by id
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                    if (category == null)
                    {
                        throw new Exception("Category not found");
                    }

                    // Asiign pre signed URL to category
                    var url = _cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImagePath);

                    return new BaseResponseDTO<CategoryResponseDTO>
                    {
                        Data = category.ToCategoryResponseDTO(url),
                        Message = "Category found",
                        Flag = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<CategoryResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }
        public async Task<BaseResponseDTO<CategoryResponseDTO>> UpdateAsync(string userId, int id, CategoryRequestDTO entity)
        {
            try
            {
                // Update category
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //Get the user and check if the user is admin
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        throw new Exception("User not found");
                    }

                    if (!await _userManager.IsInRoleAsync(user, "admin"))
                    {
                        throw new Exception("User is not an admin");
                    }

                    // Find category by id
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
                        var (flag, filePath) = await _cloudProvider.UpdateFile(entity.CategoryImage, _configuration["StorageDirectories:CategoryImages"], category.CategoryImagePath);
                        if (!flag)
                        {
                            throw new Exception("Failed to update category image");
                        }
                        category.CategoryImagePath = filePath;
                    }

                    // Update category
                    context.Categories.Update(category);
                    await context.SaveChangesAsync();

                    // Assign pre signed URL to category
                    var url = _cloudProvider.GeneratePreSignedUrlForDownload(category.CategoryImagePath);

                    // send notifications to vendors
                    await _notificationRepository.AddNotification("Category Updated", "A category has been updated",DatabaseChangeEventType.Update, "vendor", null, EntityType.Category, category.Id, null);

                    // send database change event to all clients, vendors and admins
                    await _notificationRepository.SendDatabaseChangeNotification(DatabaseChangeEventType.Update, EntityType.Category, category.Id, userId);

                    return new BaseResponseDTO<CategoryResponseDTO>
                    {
                        Message = "Category updated successfully",
                        Flag = true,
                        Data = category.ToCategoryResponseDTO(url)
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<CategoryResponseDTO>
                {
                    Message = ex.Message,
                    Flag = false
                };
            }
        }

    }
}
