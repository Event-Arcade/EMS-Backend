using System.ComponentModel.DataAnnotations.Schema;
using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.Models
{
    public class Package
    {
        public int Id { get; set; }
        public PackageStatus Status
        {
            get
            {
                if (SubPackages.All(x => x.Status == PackageStatus.Approved))
                {
                    return PackageStatus.Approved;
                }
                else if (SubPackages.Any(x => x.Status == PackageStatus.Pending))
                {
                    return PackageStatus.Pending;
                }
                else if (SubPackages.Any(x => x.Status == PackageStatus.Rejected))
                {
                    return PackageStatus.Rejected;
                }
                else if (SubPackages.All(x => x.Status == PackageStatus.Delivered))
                {
                    return PackageStatus.Delivered;
                }
                else
                {
                    return PackageStatus.Pending;
                }
            }
        }
        public DateTime CreatedAt { get; set; }
        public ICollection<SubPackage> SubPackages { get; set; } = new List<SubPackage>();
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
