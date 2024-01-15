using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs
{
    public class PlatformDTO
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Publisher { get; set; } = string.Empty;
        [Required] public string Cost { get; set; } = string.Empty;
    }
}