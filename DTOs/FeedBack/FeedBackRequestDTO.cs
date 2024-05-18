using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs
{
    public class FeedBackRequestDTO
    {
        [Required, MaxLength(500), MinLength(5)]
        public string Comment { get; set; }
        [Required, Range(0, 5)]
        public double Rating { get; set; }
        [Required]
        public int ServiceId { get; set; }
        // should not be more than 5
        [MaxLength(5)]
        public ICollection<IFormFile>? FeedBackStaticResources { get; set; }
    }
}