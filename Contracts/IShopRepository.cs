using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.Shop;
using EMS.BACKEND.API.Models;

namespace SharedClassLibrary.Contracts
{
    public interface IShopRepository
    {
        Task<BaseResponseDTO<IEnumerable<ShopResponseDTO>>> FindAllAsync();
        Task<BaseResponseDTO<ShopResponseDTO>> FindByIdAsync(int id);
        Task<BaseResponseDTO<string, ShopResponseDTO>> CreateAsync(string userId, ShopCreateDTO entity);
        Task<BaseResponseDTO<ShopResponseDTO>> UpdateAsync(string userId, int id, ShopCreateDTO entity);
        Task<BaseResponseDTO> DeleteAsync(string userId, int id);
        Task<BaseResponseDTO<ShopResponseDTO>> GetShopByVendor(string userId);
    }
}
