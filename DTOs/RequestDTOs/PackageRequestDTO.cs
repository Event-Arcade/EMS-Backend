namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class PackageRequestDTO : BaseRequestDTO
    {
        public string UserId { get; set; }
        public string Status { get; set; }
        public List<SubPackageRequestDTO> SubPackages { get; set; }
    }
}
