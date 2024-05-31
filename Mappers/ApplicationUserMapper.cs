using EMS.BACKEND.API.DTOs.Account;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Mappers
{
    public static class ApplicationUserMapper
    {
        public static UserAccountResponseDTO MapUserToUserAccountResponseDTO(this ApplicationUser user, string role)
        {
            return new UserAccountResponseDTO
            {
                Id = user.Id,
                OnlineId = user.OnlineId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Street = user.Street,
                City = user.City,
                PostalCode = user.PostalCode,
                Province = user.Province,
                Longitude = user.Longitude ?? 0,
                Latitude = user.Latitude ?? 0,
                Email = user.Email,
                Role = role,
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
                Longitude = userDTO.Longitude ?? 0,
                Latitude = userDTO.Latitude ?? 0,
                Email = userDTO.Email,
                UserName = userDTO.Email
            };
        }
    }
}