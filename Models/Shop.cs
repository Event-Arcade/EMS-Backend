using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    [Table("Shops")]
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double? Rating { get; set; }
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public string? BackgroundImagePath { get; set; }
        public virtual ICollection<ShopService>? Services { get; set; }
    }
}
