using System.ComponentModel.DataAnnotations.Schema;
using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.Models
{
    public class Package
    {
        public int Id { get; set; }
        public PackageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<SubPackage> SubPackages { get; set; } 
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
