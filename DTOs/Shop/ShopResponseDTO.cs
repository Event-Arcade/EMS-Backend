
namespace EMS.BACKEND.API.DTOs.Shop
{
    public class ShopResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double? Rating { get; set; }
        public string OwnerId { get; set; }
        public string? BackgroundImageURL { get; set; }
    }
}