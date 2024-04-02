using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace EMS.BACKEND.API.Contracts
{
    public interface ICategoryRepository
    {
        Task<BaseResponseDTO> GetAllCategories();
        Task<BaseResponseDTO> GetCategoryById(string categoryId);
        Task<BaseResponseDTO> AddCategory(BaseRequestDTO categoryRequestDTO);
        Task<BaseResponseDTO> UpdateCategory(BaseRequestDTO categoryRequestDTO);
        Task<BaseResponseDTO> DeleteCategory(string categoryId);

    }
}
