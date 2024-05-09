using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.ShopService;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IShopServiceRepository : IBaseRepository<ShopServiceResponseDTO, ShopServiceRequestDTO>
    {
        Task<BaseResponseDTO<IEnumerable<ShopService>>> GetServicesByShopId(string shopId);
    }
}
