using EMS.BACKEND.API.DTOs.Account;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace SharedClassLibrary.Contracts
{
    public interface IUserAccountRepository
    {
        Task<BaseResponseDTO<string>> CreateAccountAsync(RegisterUserDTO registerUser);
        Task<BaseResponseDTO<UserAccountResponseDTO>> GetAccountByIdAsync(string id);
        Task<BaseResponseDTO<UserAccountResponseDTO>> UpdateAccountAsync(string userId, UpdateUserDTO updateUserDTO);
        Task<BaseResponseDTO> UpdateAccountPasswordAsync(string userId, UpdatePasswordDTO updatePasswordDTO);
        Task<BaseResponseDTO> DeleteAccountAsync(string userId);
        Task<BaseResponseDTO<string>> LoginAccountAsync(LoginDTO loginDTO);
        Task<BaseResponseDTO<string>> GoogleLoginAsync(GoogleLoginDTO googleLoginDTO);
        Task<BaseResponseDTO<ICollection<UserAccountResponseDTO>>> GetAllUsersAsync(string userId);


    }
}
