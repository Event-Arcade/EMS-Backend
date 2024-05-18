using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    public class AdminStaticResource : BaseStaticResource
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string AdminId{get;set;}
        public virtual ApplicationUser Admin{get;set;}
    }
}