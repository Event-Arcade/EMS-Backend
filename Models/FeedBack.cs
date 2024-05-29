using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    [Table("FeedBacks")]
    public class FeedBack
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public DateTime PostedOn { get; set; }
        public double Rating { get; set; }
        public int ServiceId { get; set; }
        public virtual ShopService Service { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [MaxLength(5)]
        public virtual ICollection<FeedBackStaticResource>? FeedBackStaticResources { get; set; } 
    }
}
