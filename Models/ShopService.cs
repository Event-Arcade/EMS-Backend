using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    [Table("ShopServices")]
    public class ShopService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int NoOfGuests { get; set; }
        public bool Indoor { get; set; }
        public bool Outdoor { get; set; }
        public string? Description { get; set; }
        public double Rating { get; set; }
        public int ShopId { get; set; }
        public virtual Shop Shop { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<FeedBack>? FeedBacks { get; set; }
        public virtual ICollection<ShopServiceStaticResources>? ShopServiceStaticResources { get; set; } 
        public virtual ICollection<SubPackage>? SubPackages { get; set; }
    }
}
