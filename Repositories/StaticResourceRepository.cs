using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Controllers
{
    public class StaticResourceRepository(IServiceProvider serviceProvider, ICloudProviderRepository cloudProvider, IConfiguration configuration) : IStaticResourceRepository
    {
        public async Task<BaseResponseDTO<StaticResource>> GetFile(string fileId)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var file = await dbContext.StaticResources.Where(x => x.Id == fileId).FirstOrDefaultAsync();
                if (file == null)
                {
                    return new BaseResponseDTO<StaticResource>
                    {
                        Flag = false,
                        Message = "File not found"
                    };
                }

                //Generate the file url
                var fileUrl = cloudProvider.GeneratePreSignedUrlForDownload(file.ResourceUrl);
                file.ResourceUrl = fileUrl;

                return new BaseResponseDTO<StaticResource>
                {
                    Data = file,
                    Flag = true,
                    Message = "File found"
                };
            }
        }

        public async Task<BaseResponseDTO> RemoveFile(string fileId)
        {
            // Check if the fileId is empty
            if (string.IsNullOrEmpty(fileId))
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "File not found"
                };
            }

            // Get the file from the database
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var file = await dbContext.StaticResources.Where(x => x.Id == fileId).FirstOrDefaultAsync();
                if (file == null)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "File not found"
                    };
                }

                // Remove the file from the database
                dbContext.StaticResources.Remove(file);
                await dbContext.SaveChangesAsync();
                return new BaseResponseDTO
                {
                    Flag = true,
                    Message = "File removed"
                };
            }
        }

        public async Task<BaseResponseDTO> UpdateFile(IFormFile formFile, string fileId)
        {
            // Check if the fileId or file is empty
            if (string.IsNullOrEmpty(fileId) || formFile == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "File not found"
                };
            }

            // Get the file from the database
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var file = await dbContext.StaticResources.Where(x => x.Id == fileId).FirstOrDefaultAsync();
                if (file == null)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "File not found"
                    };
                }

                // Update the file in the database
                var previousFile = file.ResourceUrl;
                var (result, path) = await cloudProvider.UploadFile(formFile,configuration["StorageDirectories:AdminStaticResources"]);
                if (!result)
                {
                    return new BaseResponseDTO
                    {
                        Flag = false,
                        Message = "File not uploaded"
                    };
                }
                file.ResourceUrl = path;

                // Remove the previous file from the cloud
                await cloudProvider.RemoveFile(previousFile);

                // Save the changes
                dbContext.Update(file);
                await dbContext.SaveChangesAsync();
                return new BaseResponseDTO
                {
                    Flag = true,
                    Message = "File updated"
                };
            }

        }

        public async Task<BaseResponseDTO> UploadFile(IFormFile file)
        {
            // Check if the file is empty
            if (file == null)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "File not found"
                };
            }

            // Upload the file to the cloud
            var (result, path) = await cloudProvider.UploadFile(file, configuration["StorageDirectories:AdminStaticResources"]);
            if (!result)
            {
                return new BaseResponseDTO
                {
                    Flag = false,
                    Message = "File not uploaded"
                };
            }

            // Save the file to the database
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var staticResource = new StaticResource
                {
                    Id = Guid.NewGuid().ToString(),
                    ResourceUrl = path
                };
                await dbContext.StaticResources.AddAsync(staticResource);
                await dbContext.SaveChangesAsync();
                return new BaseResponseDTO
                {
                    Flag = true,
                    Message = "File uploaded"
                };

            }
        }
    }
}