using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.DTOs.SubPackage
{
    public class SubPackageRequestDTO
    {
        public string Description { get; set; }
        public int PackageId { get; set; }
        public int ServiceId { get; set; }
        public PackageStatus Status { get; set; }
    }
}