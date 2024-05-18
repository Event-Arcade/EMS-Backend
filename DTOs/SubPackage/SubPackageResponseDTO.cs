using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public class SubPackageResponseDTO
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public int ServiceId { get; set; }
        public PackageStatus Status { get; set; }
    }
}
