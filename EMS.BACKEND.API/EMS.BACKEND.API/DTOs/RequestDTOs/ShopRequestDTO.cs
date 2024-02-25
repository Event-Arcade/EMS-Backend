using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class ShopRequestDTO
    {
        [Required] public string Id { get; set; } = string.Empty;
        [Required] public string Name{ get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public double Rating { get; set; }
    }
}
