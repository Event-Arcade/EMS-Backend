using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class ShopRequestDTO
    {
        public string Id { get; set; }
        [Required] public string Name{ get; set; } 
        [Required] public string Description { get; set; } 
        [Required] public double Rating { get; set; }
    }
}
