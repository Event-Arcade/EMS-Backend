using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface ICategoryRepository
    {
        Task<BaseResponseDTO<List<Category>>> GetAllCategories();
        Task<BaseResponseDTO<Category>> GetCategoryById(string categoryId);
        Task<BaseResponseDTO> AddCategory(BaseRequestDTO categoryRequestDTO);
        Task<BaseResponseDTO> UpdateCategory(BaseRequestDTO categoryRequestDTO);
        Task<BaseResponseDTO> DeleteCategory(string categoryId);

    }
}
