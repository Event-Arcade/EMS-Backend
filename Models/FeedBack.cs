using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    public class FeedBack
    {
        public string Id { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public double Rating { get; set; }
        public string ServiceId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User{get; set;}
        public string FeedbackStaticResourcePath { get; set; }
        [NotMapped]
        public IFormFile FeedbackImage { get; set; }
    }
}
