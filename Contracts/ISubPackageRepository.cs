using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface ISubPackageRepository : IBaseRepository<SubPackage,SubPackage>
    {
        Task<BaseResponseDTO<IEnumerable<SubPackage>>> GetSubpackagesByUser (string userId);   
    }
}