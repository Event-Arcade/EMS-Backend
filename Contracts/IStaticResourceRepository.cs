namespace EMS.BACKEND.API.Contracts
{
    public interface IStaticResourceRepository
    {
        //upload a Resource
        Task<(bool, string)> UploadFile(IFormFile file);
        //remove a Resource
        Task<bool> RemoveFile(string fileId);
        //update a Resource
        Task<(bool, string)> UpdateFile(IFormFile file, string fileId);
    }
}
