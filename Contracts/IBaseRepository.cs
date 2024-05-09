using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace EMS.BACKEND.API.Contracts
{
    public interface IBaseRepository<T, E> where T : class where E : class
    {
        Task<BaseResponseDTO<IEnumerable<T>>> FindAllAsync();
        Task<BaseResponseDTO<T>> FindByIdAsync(int id);
        Task<BaseResponseDTO<T>> CreateAsync(string userId, E entity);
        Task<BaseResponseDTO<T>> UpdateAsync(string userId, int id, E entity);
        Task<BaseResponseDTO> DeleteAsync(string userId, int id);
    }
}