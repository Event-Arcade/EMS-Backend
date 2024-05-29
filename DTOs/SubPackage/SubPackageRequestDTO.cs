using System.ComponentModel.DataAnnotations;
using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.DTOs.SubPackage
{
    public class SubPackageRequestDTO
    {
        public int? Id { get; set; }
        public int? PackageId { get; set; }
        [Required]
        public DateTime OrderTime { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int ServiceId { get; set; }
        public PackageStatus Status { get; set; }
    }
}