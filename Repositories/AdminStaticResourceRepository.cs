using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Controllers
{
    public class AdminStaticResourceRepository(IServiceProvider serviceProvider, ICloudProviderRepository cloudProvider, IConfiguration configuration) : IAdminStaticResourceRepository
    {
        public async Task<BaseResponseDTO<String>> CreateAsync(AdminStaticResource entity)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // Check if the entity is null
                    if (entity == null)
                    {
                        throw new ArgumentNullException(nameof(entity));
                    }

                    // Check if the entity is null
                    if (entity.Resource == null)
                    {
                        throw new ArgumentNullException(nameof(entity.Resource));
                    }

                    // Upload the file to the cloud
                    var (flag, path) = await cloudProvider.UploadFile(entity.Resource, configuration["StorageDirectories:AdminStaticResources"]);
                    if (!flag)
                    {
                        throw new Exception("Failed to upload the file to the cloud");
                    }

                    // Save the entity to the database
                    entity.Id = Guid.NewGuid().ToString();
                    entity.ResourceUrl = path;
                    await dbContext.AdminStaticResources.AddAsync(entity);
                    await dbContext.SaveChangesAsync();

                    return new BaseResponseDTO<String> { Message = "Resource uploaded successfully", Flag = true };
                }


            }
            catch (Exception ex)
            {
                return new BaseResponseDTO <String>{ Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<String>> DeleteAsync(string id)
        {
            try
            {
                // Check if the id is null
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var entity = dbContext.AdminStaticResources.FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                    {
                        throw new Exception("Resource not found");
                    }

                    // Delete the file from the cloud
                    var flag = await cloudProvider.RemoveFile(entity.ResourceUrl);
                    if (!flag)
                    {
                        throw new Exception("Failed to delete the file from the cloud");
                    }

                    // Delete the entity from the database
                    dbContext.AdminStaticResources.Remove(entity);
                    await dbContext.SaveChangesAsync();

                    return new BaseResponseDTO<String> { Message = "Resource deleted successfully", Flag = true };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<String> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<IEnumerable<AdminStaticResource>>> FindAllAsync()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var entities = await dbContext.AdminStaticResources.ToListAsync();
                    //convert path to url
                    foreach (var entity in entities)
                    {
                        var url = cloudProvider.GeneratePreSignedUrlForDownload(entity.ResourceUrl);
                        entity.ResourceUrl = url;
                    }

                    return new BaseResponseDTO<IEnumerable<AdminStaticResource>> { Data = entities, Message = "Successfully found", Flag = true };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<IEnumerable<AdminStaticResource>> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO<AdminStaticResource>> FindByIdAsync(string id)
        {
            try{
                // Check if the id is null
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var entity = dbContext.AdminStaticResources.FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                    {
                        throw new Exception("Resource not found");
                    }

                    //convert path to url
                    var url = cloudProvider.GeneratePreSignedUrlForDownload(entity.ResourceUrl);
                    entity.ResourceUrl = url;

                    return new BaseResponseDTO<AdminStaticResource> { Data = entity, Message = "Successfully found", Flag = true };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO<AdminStaticResource> { Message = ex.Message, Flag = false };
            }
        }
        public async Task<BaseResponseDTO> UpdateAsync(String id,AdminStaticResource entity)
        {
            try{
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // Check if the entity is null
                    if (entity == null)
                    {
                        throw new ArgumentNullException(nameof(entity));
                    }

                    // Check if the entity is null
                    if (entity.Resource == null)
                    {
                        throw new ArgumentNullException(nameof(entity.Resource));
                    }

                    // Update the file to the cloud
                    var (flag, path) = await cloudProvider.UpdateFile(entity.Resource, configuration["StorageDirectories:AdminStaticResources"], entity.ResourceUrl);
                    if (!flag)
                    {
                        throw new Exception("Failed to upload the file to the cloud");
                    }

                    // Save the entity to the database
                    entity.ResourceUrl = path;
                    dbContext.AdminStaticResources.Update(entity);
                    dbContext.SaveChanges();

                    return new BaseResponseDTO { Message = "Resource updated successfully", Flag = true };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Message = ex.Message, Flag = false };
            }
        }
    }
}