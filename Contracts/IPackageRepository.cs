using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IPackageRepository
    {
        Task<PackageListResponse> GetAllPackages();
        Task<PackageResponse> GetPackageById(string id);
        Task<PackageResponse> CreatePackage(PackageRequestDTO packageRequestDTO);
        Task<PackageResponse> UpdatePackage(PackageRequestDTO packageRequestDTO);
        Task<GeneralResponse> DeletePackage(string id);
    }
}
