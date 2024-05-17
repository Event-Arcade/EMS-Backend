using EMS.BACKEND.API.DTOs.Shop;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Mappers
{
    public static class ShopMapper
    {
        public static Shop MapToShop(this ShopCreateDTO shopCreateDTO, ApplicationUser user, string imagePath)
        {
            return new Shop
            {
                Name = shopCreateDTO.Name,
                Description = shopCreateDTO.Description,
                OwnerId = user.Id,
                BackgroundImagePath = imagePath
            };
        }

        public static ShopResponseDTO MapToShopResponseDTO(this Shop shop, string imageURL)
        {
            return new ShopResponseDTO
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
                Rating = shop.Rating,
                OwnerId = shop.OwnerId,
                BackgroundImageURL = imageURL
            };
        }
    }
}