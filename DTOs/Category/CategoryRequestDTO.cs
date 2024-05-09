using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.Category
{
    public class CategoryRequestDTO
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
        [StringLength(200)]
        public string? Description { get; set; }
        public IFormFile? CategoryImage { get; set; }
    }
}