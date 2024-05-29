using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public class ShopServiceResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
        public int NoOfGuests { get; set; }
        public bool Indoor { get; set; }
        public bool Outdoor { get; set; }
        public int ShopId { get; set; }
        public int CategoryId { get; set; }
        public double Rating { get; set; }
        public ICollection<string>? ShopServiceStaticResourcesURLs { get; set; }
    }
}
