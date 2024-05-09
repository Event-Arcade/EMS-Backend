namespace EMS.BACKEND.API.DTOs.Category
{
    public class CategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CategoryImagePath { get; set; }
    }
}