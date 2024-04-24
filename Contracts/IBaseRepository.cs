using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace EMS.BACKEND.API.Contracts
{
    public interface IBaseRepository<T,E> where T : class where E : class
    {
        Task<BaseResponseDTO<IEnumerable<T>>> FindAllAsync();
        Task<BaseResponseDTO<T>> FindByIdAsync(string id);
        Task<BaseResponseDTO> CreateAsync(E entity);
        Task<BaseResponseDTO> UpdateAsync(string id,E entity);
        Task<BaseResponseDTO> DeleteAsync(string id);
    }
}