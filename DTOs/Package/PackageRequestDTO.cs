using EMS.BACKEND.API.DTOs.SubPackage;
namespace EMS.BACKEND.API.DTOs.Package
{
    public class PackageRequestDTO
    {
        public int? Id { get; set; }
        public string? UserId { get; set; }
        public ICollection<SubPackageRequestDTO> SubPackages { get; set; }
    }
}