using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.ShopService
{
    public class ShopServiceRequestDTO
    {
        [Required, StringLength(20)]
        public string Name { get; set; }
        [Required, Range(0, double.MaxValue)]
        public double Price { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int NoOfGuests { get; set; }
        [Required]
        public bool Indoor { get; set; }
        [Required]
        public bool Outdoor { get; set; }
        [StringLength(100)]
        public string? Description { get; set; }
        [Required]
        public int ShopId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public ICollection<IFormFile>? ShopServiceStaticResources { get; set; }
    }
}