using EMS.BACKEND.API.DTOs.Category;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Mappers
{
    public static class CategoryMapper
    {
        public static CategoryResponseDTO ToCategoryResponseDTO(this Category category, string categoryImageUrl)
        {
            return new CategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CategoryImagePath = categoryImageUrl
            };
        }

        public static Category ToCategory(this CategoryRequestDTO categoryRequestDTO, string categoryImagePath, ApplicationUser user)
        {
            return new Category
            {
                Name = categoryRequestDTO.Name,
                Description = categoryRequestDTO.Description,
                CategoryImagePath = categoryImagePath,
                AdminId = user.Id
            };
        }
    }
}