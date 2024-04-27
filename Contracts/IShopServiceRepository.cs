using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace SharedClassLibrary.Contracts
{
    public interface IShopServiceRepository : IBaseRepository<Shop,Shop>
    {
        Task<BaseResponseDTO<Shop>> GetShopByServiceId(string serviceId);
        Task<BaseResponseDTO<Shop>> GetShopByVendor();
    }
}
