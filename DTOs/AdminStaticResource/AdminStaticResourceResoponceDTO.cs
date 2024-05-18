namespace EMS.BACKEND.API.DTOs.AdminStaticResource
{
    public class AdminStaticResourceResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ResourceUrl { get; set; }
        public string AdminId { get; set; }
    }
}