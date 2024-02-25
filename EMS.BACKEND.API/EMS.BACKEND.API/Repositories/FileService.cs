using EMS.BACKEND.API.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class FileService(IWebHostEnvironment hostEnvironment) : IFileService
    {
        public async Task<(bool condition, byte[] archiveData, string message)> DownloadFiles(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var fileBytes =await File.ReadAllBytesAsync(filePath);
                    //var fileStream = new MemoryStream(fileBytes);
                    //var file = new FormFile(fileStream, 0, fileBytes.Length, null, Path.GetFileName(filePath));
                    return (true, fileBytes,Path.GetExtension(filePath)); // Adjust the MIME type according to your image format
                }
                else
                {
                    return (false,null!, "Image not found.");
                }
            }
            catch (Exception ex)
            {
                return (false,null!, $"Internal server error: {ex}");
            }
        }
        public async Task<(bool, string)> UploadFile(IFormFile file, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(hostEnvironment.ContentRootPath, subDirectory);

            Directory.CreateDirectory(target);

            try
            {
                if (file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() ;
                    var filePath = Path.Combine(hostEnvironment.WebRootPath, "Images", fileName + Path.GetExtension(file.FileName));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return (true, filePath);
                }
                return (false, "No file uploaded.");
            }
            catch (Exception ex)
            {
                return (false, $"Internal server error: {ex}");
            }
        }

    }
}
