using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace EMS.BACKEND.API.Contracts
{
    public interface ICategoryRepository
    {
        Task<ResponseDTO> GetAllCategories();
        Task<ResponseDTO> GetCategoryById(string categoryId);
        Task<ResponseDTO> AddCategory(RequestDTO categoryRequestDTO);
        Task<ResponseDTO> UpdateCategory(RequestDTO categoryRequestDTO);
        Task<ResponseDTO> DeleteCategory(string categoryId);

    }
}
