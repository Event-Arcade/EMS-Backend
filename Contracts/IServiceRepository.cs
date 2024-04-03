using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IServiceRepository : IBaseRepository<Service, Service>
    {
        Task<BaseResponseDTO<IEnumerable<Service>>> GetServicesByShopId(string shopId);
    }
}
