using EMS.BACKEND.API.DTOs.RequestDTOs;
using static EMS.BACKEND.API.DTOs.ResponseDTOs.Responses;

namespace SharedClassLibrary.Contracts
{
    public interface IShopServiceRepository
    {
        Task<GeneralResponse> CreateShop(ShopRequestDTO shopRequestDTO);
        Task<GeneralResponse> UpdateShop(ShopRequestDTO shopRequestDTO);
        Task<GeneralResponse> DeleteShop(ShopRequestDTO shopRequestDTO);
        Task<GeneralResponse> GetShop();
        Task<GeneralResponse> AddNewService(ServiceRequestDTO serviceRequestDTO);
        Task<GeneralResponse> UpdateService(ServiceRequestDTO serviceRequestDTO);
        Task<GeneralResponse> DeleteService(ServiceRequestDTO serviceRequestDTO);
        Task<GeneralResponse> GetService(ServiceRequestDTO serviceRequestDTO);
    }
}
