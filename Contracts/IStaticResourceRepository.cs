using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IStaticResourceRepository
    {
        //upload a Resource
        Task<BaseResponseDTO> UploadFile(IFormFile formFile);
        //remove a Resource
        Task<BaseResponseDTO> RemoveFile(string fileId);
        //update a Resource
        Task<BaseResponseDTO> UpdateFile(IFormFile file, string fileId);
        //get a Resource
        Task<BaseResponseDTO<StaticResource>> GetFile(string fileId);
    }
}
