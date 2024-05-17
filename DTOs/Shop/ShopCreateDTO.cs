using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.Shop
{   
    public class ShopCreateDTO
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public IFormFile? BackGroundImage { get; set; }
    }
}