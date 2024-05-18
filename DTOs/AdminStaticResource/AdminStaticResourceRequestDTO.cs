namespace EMS.BACKEND.API.DTOs.AdminStaticResource
{
    public class AdminStaticResourceRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Resource { get; set; }
    }
}