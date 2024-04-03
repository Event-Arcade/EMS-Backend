using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.Models
{
    public class SubPackage
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string PackageId { get; set; }
        public PackageStatus Status { get; set; }
        public  string ServiceId { get; set; }

    }
}
