namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class CategoryRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile CategoryImage { get; set; }
    }
}