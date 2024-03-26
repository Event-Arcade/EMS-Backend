
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace SharedClassLibrary.Contracts
{
    public interface IUserAccountRepository
    {
        Task<LoginResponse> CreateAccount(UserRequestDTO userDTO);
        Task<GeneralResponse> UpdateAccount(UserRequestDTO userDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
        Task<UserResponse> GetMe();
        string GeneratePreSignedUrl();
    }
}
