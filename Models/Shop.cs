using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.Models
{
    public class Shop
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public string OwnerId { get; set; }
        public virtual ICollection<Service> Services  { get; set; }
    }
}
