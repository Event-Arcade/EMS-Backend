using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace EMS.BACKEND.API.Contracts
{
    public interface ICategoryRepository
    {
        Task<CategoryListResponse> GetAllCategories();
        Task<CategoryResponse> GetCategoryById(string categoryId);
        Task<CategoryResponse> AddCategory(CategoryRequestDTO categoryRequestDTO);
        Task<CategoryResponse> UpdateCategory(CategoryRequestDTO categoryRequestDTO);
        Task<GeneralResponse> DeleteCategory(string categoryId);

    }
}
