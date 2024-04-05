using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IPackageRepository : IBaseRepository<Package,PackageRequestDTO>
    {
        Task<BaseResponseDTO<IEnumerable<Package>>> GetAllPackagesByUser(string userId);
    }
}
