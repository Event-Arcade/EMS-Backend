using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    public class AdminStaticResource : BaseStaticResource
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public IFormFile Resource { get; set; }
        public string UserId{get;set;}
        public virtual ApplicationUser User{get;set;}
    }
}