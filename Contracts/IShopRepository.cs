using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.Shop;
using EMS.BACKEND.API.Models;

namespace SharedClassLibrary.Contracts
{
    public interface IShopRepository : IBaseRepository<ShopResponseDTO, ShopCreateDTO>
    {
        Task<BaseResponseDTO<ShopResponseDTO>> GetShopByVendor(string userId);
    }
}
