using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.ShopService;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Mappers
{
    public static class ShopServiceMapper
    {
        public static ShopServiceResponseDTO ToShopServiceResponseDTO(this ShopService shopService, string shopServiceOwner, ICollection<string> resourceUrls)
        {
            return new ShopServiceResponseDTO
            {
                Id = shopService.Id,
                Name = shopService.Name,
                Price = shopService.Price,
                Description = shopService.Description,
                NoOfGuests = shopService.NoOfGuests,
                Indoor = shopService.Indoor,
                Outdoor = shopService.Outdoor,
                ShopId = shopService.ShopId,
                CategoryId = shopService.CategoryId,
                Rating = shopService.Rating,
                ShopServiceOwner = shopServiceOwner,
                ShopServiceStaticResourcesURLs = resourceUrls
            };
        }
        public static ShopService ToShopService(this ShopServiceRequestDTO shopServiceRequestDTO)
        {
            return new ShopService
            {
                Name = shopServiceRequestDTO.Name,
                Price = shopServiceRequestDTO.Price,
                NoOfGuests = shopServiceRequestDTO.NoOfGuests,
                Indoor = shopServiceRequestDTO.Indoor,
                Outdoor = shopServiceRequestDTO.Outdoor,
                Description = shopServiceRequestDTO.Description,
                ShopId = shopServiceRequestDTO.ShopId,
                CategoryId = shopServiceRequestDTO.CategoryId
            };
        }
    }
}