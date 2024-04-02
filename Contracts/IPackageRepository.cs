using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IPackageRepository
    {
        Task<ResponseDTO<List<Package>>> GetAllPackages();
        Task<BaseResponseDTO> GetPackageById(string id);
        Task<BaseResponseDTO> CreatePackage(PackageRequestDTO packageRequestDTO);
        Task<BaseResponseDTO> UpdatePackage(PackageRequestDTO packageRequestDTO);
        Task<BaseResponseDTO> DeletePackage(string id);
    }
}
