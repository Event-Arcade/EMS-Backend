using Amazon.S3;
using Amazon.S3.Model;
using EMS.BACKEND.API.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class FileService(IAmazonS3 _s3Client) : IFileService
    {
        public async Task<(bool, string)> UploadFile(IFormFile file, string subDirectory)
        {
            subDirectory = subDirectory ?? "defalut";

            try
            {
                if (file.Length > 0)
                {
                    string filePath = $"{subDirectory}/{Guid.NewGuid().ToString()}";
                    var request = new PutObjectRequest()
                    {
                        BucketName = "ems-static-data-storage",
                        Key = filePath,
                        InputStream = file.OpenReadStream(),
                    };

                    request.Metadata.Add("content-Type", file.ContentType);
                    await _s3Client.PutObjectAsync(request);
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
