using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using EMS.BACKEND.API.Contracts;
using Microsoft.CodeAnalysis.FlowAnalysis;
using NuGet.Protocol;

namespace EMS.BACKEND.API.Repositories
{
    public class CloudProviderRepository : ICloudProviderRepository
    {
        private readonly IConfiguration configuration;
        private readonly IAmazonS3 amazonS3;
        public CloudProviderRepository(IConfiguration configuration, IAmazonS3 amazonS3)
        {
            configuration = configuration;
            amazonS3 = amazonS3;
        }
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
        public async Task<(bool, string)> UploadFile(IFormFile? file, string subDirectory)
        {
            try
            {
                string filePath;
                //check if file is null
                if (file == null)
                {
                    //assign default image path
                    filePath = $"{subDirectory}/default.png";
                    return (true, filePath);

                }
                else
                {
                    //generate file path
                    filePath = $"{subDirectory}/{Guid.NewGuid().ToString()}";
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
                }


                return (false, "Failed to upload file");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<bool> RemoveFile(string filePath)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = configuration["AWS:BucketName"],
                    Key = filePath
                };

                var result = await amazonS3.DeleteObjectAsync(request);
                if (result.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                Console.WriteLine(result.ToJson());
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<(bool, string)> UpdateFile(IFormFile file, string subDirectory, string oldFilePath)
        {
            try
            {
                //remove old file
                await RemoveFile(oldFilePath);

                //upload new file
                return await UploadFile(file, subDirectory);

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}