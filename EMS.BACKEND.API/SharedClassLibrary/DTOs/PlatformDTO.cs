using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs
{
    public class PlatformDTO
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public Double Price { get; set; } =0;
        [Required] public Double SuppliedAreaRange { get; set; } = 0;
        [Required] public int MaxHeadCount { get; set; } = 0;
        [Required] public Double Longitude { get; set; } = 0;
        [Required] public Double Latitude { get; set; } = 0;
        [Required] public string ContactNumber { get; set; } = string.Empty;
    }
}