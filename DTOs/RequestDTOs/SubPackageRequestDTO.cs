using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class SubPackageRequestDTO 
    {
        public string? PackageId { get; set; }
        public string? serviceId { get; set; }
        public PackageStatus Status { get; set; }
    }
}
