using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace SharedClassLibrary.Contracts
{
    public interface IUserAccountRepository
    {
        Task<BaseResponseDTO<String>> CreateAccount(ApplicationUser userDTO);
        Task<BaseResponseDTO> UpdateAccount(ApplicationUser userDTO);
        Task<BaseResponseDTO<String>> LoginAccount(ApplicationUser userDTO);
        Task<BaseResponseDTO<UserResponseDTO>> GetMe();
        Task<BaseResponseDTO> DeleteAccount(string userId);
    }
}
