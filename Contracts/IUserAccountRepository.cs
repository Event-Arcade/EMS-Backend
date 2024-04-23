﻿
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace SharedClassLibrary.Contracts
{
    public interface IUserAccountRepository
    {
        Task<BaseResponseDTO<String>> CreateAccount(UserRequestDTO userDTO);
        Task<BaseResponseDTO> UpdateAccount(UpdateUserRequestDTO userDTO);
        Task<BaseResponseDTO<String>> LoginAccount(LoginDTO loginDTO);
        Task<BaseResponseDTO<UserResponseDTO>> GetMe();
    }
}
