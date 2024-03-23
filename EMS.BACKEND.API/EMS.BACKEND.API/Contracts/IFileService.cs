namespace EMS.BACKEND.API.Contracts
{
    public interface IFileService
    {
        Task<(bool, string)> UploadFile(IFormFile file, string subDirectory);
        Task<(bool condition, Stream staticData,string contentType, string message)> DownloadFiles(string filePath);
    }
}
