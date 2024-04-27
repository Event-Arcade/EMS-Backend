using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IPackageRepository : IBaseRepository<Package,Package>
    {
        Task<BaseResponseDTO<IEnumerable<Package>>> GetAllPackagesByUser(string userId);
    }
}
