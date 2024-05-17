using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.ShopService;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Mappers
{
    public static class ShopServiceMapper
    {
        public static ShopServiceResponseDTO ToShopServiceResponseDTO(this ShopService shopService)
        {
            return new ShopServiceResponseDTO
            {
                Id = shopService.Id,
                Name = shopService.Name,
                Price = shopService.Price,
                Description = shopService.Description,
                ShopId = shopService.ShopId,
                CategoryId = shopService.CategoryId,
                Rating = shopService.Rating,
                FeedBacks = shopService.FeedBacks,
            };
        }
        public static ShopService ToShopService(this ShopServiceRequestDTO shopServiceRequestDTO)
        {
            return new ShopService
            {
                Name = shopServiceRequestDTO.Name,
                Price = shopServiceRequestDTO.Price,
                Description = shopServiceRequestDTO.Description,
                ShopId = shopServiceRequestDTO.ShopId,
                CategoryId = shopServiceRequestDTO.CategoryId
            };
        }
    }
}