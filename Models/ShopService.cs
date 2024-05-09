using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    [Table("ShopServices")]
    public class ShopService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<FeedBack> FeedBacks { get; set; } = new List<FeedBack>();
        public ICollection<ShopServiceStaticResources> ShopServiceStaticResources { get; set; } = new List<ShopServiceStaticResources>();
    }
}
