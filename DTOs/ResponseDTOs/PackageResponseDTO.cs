namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public class PackageResponseDTO
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SubPackageResponseDTO> SubPackages { get; set; }
    }
}
