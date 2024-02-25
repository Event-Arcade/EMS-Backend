using EMS.BACKEND.API.DTOs.RequestDTOs;
using NuGet.Common;
using static EMS.BACKEND.API.DTOs.ResponseDTOs.Responses;
namespace SharedClassLibrary.Contracts
{
    public interface IUserAccountRepository
    {
        Task<LoginResponse> CreateAccount(UserRequestDTO userDTO);
        Task<GeneralResponse> UpdateAccount(UserRequestDTO userDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
        Task<UserResponse> GetMe();
    }
}
