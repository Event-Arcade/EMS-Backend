using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EMS.BACKEND.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Province { get; set; }
        public Double? Rating { get; set; }
        public Double? Longitude { get; set; }
        public Double? Latitude { get; set; }
        public string? ProfilePicturePath { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Shop>? Shops { get; set; }
        public virtual ICollection<AdminStaticResource>? AdminStaticResources { get; set; }
        public virtual ICollection<FeedBack>? FeedBacks { get; set; }
        public virtual ICollection<ChatMessage>? SentMessages { get; set; }
        public virtual ICollection<ChatMessage>? RecievedMessages { get; set; }
        public virtual ICollection<Package>? Packages { get; set; }
        public virtual ICollection<Category>? Categories { get; set; }

    }
}
