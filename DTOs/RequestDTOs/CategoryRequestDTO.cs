namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class CategoryRequestDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description {get;set;}
        public IFormFile CategoryImage {get; set;}
    }
}
