using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace SharedClassLibrary.Contracts
{
    public interface IShopServiceRepository
    {
        Task<ShopResponse> CreateShop(ShopRequestDTO shopRequestDTO);
        Task<ShopResponse> UpdateShop(ShopRequestDTO shopRequestDTO);
        Task<GeneralResponse> DeleteShop();
        Task<ShopResponse> GetMyShop();
        Task<ShopListResponse> GetAllShops();
    }
}
