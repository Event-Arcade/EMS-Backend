namespace EMS.BACKEND.API.Contracts
{
    public interface ICloudProviderRepository
    {
        //upload a file to the cloud
        Task<(bool, string)> UploadFile(IFormFile file, string subDirectory);

        //generates a presigned url for the client to download a file from the cloud
        string GeneratePreSignedUrlForDownload();
    }
}
