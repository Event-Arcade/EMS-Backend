using EMS.BACKEND.API.DTOs.SubPackage;
namespace EMS.BACKEND.API.DTOs.Package
{
    public class PackageRequestDTO
    {
        public ICollection<SubPackageRequestDTO>? SubPackages { get; set; }
    }
}