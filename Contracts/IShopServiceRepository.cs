using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace SharedClassLibrary.Contracts
{
    public interface IShopServiceRepository
    {
        Task<BaseResponseDTO> CreateShop(ShopRequestDTO shopRequestDTO);
        Task<BaseResponseDTO> UpdateShop(ShopRequestDTO shopRequestDTO);
        Task<BaseResponseDTO> DeleteShop();
        Task<BaseResponseDTO<Shop>> GetMyShop();
        Task<BaseResponseDTO<List<Shop>>> GetAllShops();
    }
}
