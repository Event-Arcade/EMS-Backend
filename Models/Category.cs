using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    [Table("Categories")]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CategoryImagePath { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ShopService> ShopServices { get; set; } = new List<ShopService>();

    }
}
