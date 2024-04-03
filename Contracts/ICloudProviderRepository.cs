namespace EMS.BACKEND.API.Contracts
{
    public interface ICloudProviderRepository
    {
        //upload a file to the cloud
        Task<(bool, string)> UploadFile(IFormFile file, string subDirectory);

        //generates a presigned url for the client to download a file from the cloud
        string GeneratePreSignedUrlForDownload(string key);

        //remove file from 
        Task<bool> RemoveFile(string filePath);

        //update file in the cloud
        Task<(bool, string)> UpdateFile(IFormFile file, string subDirectory, string oldFilePath);
    }
}
