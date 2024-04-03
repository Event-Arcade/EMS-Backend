using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class PackageRequestDTO 
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual IEnumerable<SubPackage> SubPackages { get; set; }
        public  string UserId { get; set; }
    }
}
