using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class ShopRequestDTO 
    {
        public string? Rating { get; set; }
        public string? OwnerId { get; set; }
    }
}
