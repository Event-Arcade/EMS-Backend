using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Enums;
namespace EMS.BACKEND.API.DTOs.Package
{
    public class PackageResponseDTO
    {
        public int Id { get; set; }
        public PackageStatus Status { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<SubPackageResponseDTO>? SubPackages { get; set; }
    }
}