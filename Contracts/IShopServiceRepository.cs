using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace SharedClassLibrary.Contracts
{
    public interface IShopServiceRepository
    {
        Task<BaseResponseDTO> CreateShop(ShopRequestDTO shopRequestDTO);
        Task<BaseResponseDTO> UpdateShop(ShopRequestDTO shopRequestDTO);
        Task<BaseResponseDTO> DeleteShop();
        Task<BaseResponseDTO> GetMyShop();
        Task<BaseResponseDTO> GetAllShops();
    }
}
