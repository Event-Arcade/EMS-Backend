using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    public class Shop
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public string OwnerId { get; set; }
        [NotMapped]
        public IFormFile BackgroundImage { get; set; }
        public string BackgroundImagePath { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
