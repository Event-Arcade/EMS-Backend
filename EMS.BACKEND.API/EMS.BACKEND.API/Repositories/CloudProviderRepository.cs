using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using EMS.BACKEND.API.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class CloudProviderRepository(IConfiguration configuration, IAmazonS3 amazonS3) : ICloudProviderRepository
    {
        public string GeneratePreSignedUrlForDownload(string filepath)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = configuration["AWS:BucketName"],
                Key = filepath,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddDays(1),

            };

            string url = amazonS3.GetPreSignedURL(request);
            return url;
        }

        public async Task<(bool, string)> UploadFile(IFormFile file, string subDirectory)
        {
            try
            {
                string filePath = $"{subDirectory}/{Guid.NewGuid().ToString()}";
                var request = new PutObjectRequest
                {
                    BucketName = configuration["AWS:BucketName"],
                    Key = filePath,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType
                };

                var result = await amazonS3.PutObjectAsync(request);
                if (result.HttpStatusCode == HttpStatusCode.OK)
                {
                    return (true, filePath);
                }
                return (false, "Failed to upload file");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}