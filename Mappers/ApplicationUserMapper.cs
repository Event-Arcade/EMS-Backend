using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.Account;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using EMS.BACKEND.API.Repositories;

namespace EMS.BACKEND.API.Mappers
{
    public static class ApplicationUserMapper
    {
        public static UserAccountResponseDTO MapUserToUserAccountResponseDTO(this ApplicationUser user)
        {
            return new UserAccountResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Street = user.Street,
                City = user.City,
                PostalCode = user.PostalCode,
                Province = user.Province,
                Longitude = user.Longitude,
                Latitude = user.Latitude,
                Email = user.Email,
                ProfilePictureURL = user.ProfilePicturePath
            };
        }

        public static ApplicationUser MapRegisterUserDTOToApplicationUser(this RegisterUserDTO userDTO)
        {
            return new ApplicationUser
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Street = userDTO.Street,
                City = userDTO.City,
                PostalCode = userDTO.PostalCode,
                Province = userDTO.Province,
                Longitude = userDTO.Longitude,
                Latitude = userDTO.Latitude,
                Email = userDTO.Email,
                UserName = userDTO.Email
            };
        }
    }
}