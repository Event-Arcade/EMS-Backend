using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class ServiceRequestDTO
    {
        [Required] public string Id { get; set; } = string.Empty;
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public double Price { get; set; } = 0;
        [Required] public double Longitude { get; set; } = 0;
        [Required] public double Latitude { get; set; } = 0;
        [Required, NotNull] public string ShopId { get; set; }
    }
}