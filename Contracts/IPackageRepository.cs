using EMS.BACKEND.API.DTOs.Package;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.SubPackage;

namespace EMS.BACKEND.API.Contracts
{
    public interface IPackageRepository : IBaseRepository<PackageResponseDTO, PackageRequestDTO>
    {
        Task<BaseResponseDTO<PackageResponseDTO>> UpdateSubPackage(string userId, int id, SubPackageRequestDTO subPackageRequest);
        Task<BaseResponseDTO<ICollection<SubPackageResponseDTO>>> GetSubPackages(string userId);
    }
}
