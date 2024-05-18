using EMS.BACKEND.API.DTOs.Package;
using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace EMS.BACKEND.API.Contracts
{
    public interface IPackageRepository : IBaseRepository<PackageResponseDTO, PackageRequestDTO>
    {
    }
}
