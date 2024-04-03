
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace SharedClassLibrary.Contracts
{
    public interface IUserAccountRepository
    {
        Task<BaseResponseDTO> CreateAccount(UserRequestDTO userDTO);
        Task<BaseResponseDTO> UpdateAccount(UserRequestDTO userDTO);
        Task<BaseResponseDTO> LoginAccount(LoginDTO loginDTO);
        Task<BaseResponseDTO<ApplicationUser>> GetMe();
    }
}
