using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EMS.BACKEND.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Province { get; set; }
        public Double? Rating { get; set; } = 0;
        public Double? Longitude { get; set; }
        public Double? Latitude { get; set; }
        public string? ProfilePicturePath { get; set; }
        [NotMapped]
        public IFormFile? ProfilePicture { get; set; }
        [NotMapped]
        public string? Password { get; set; }
        public virtual ICollection<Shop>? Shops { get; set; }

    }
}
