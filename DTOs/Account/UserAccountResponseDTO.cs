using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public class UserAccountResponseDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Province { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? ProfilePictureURL { get; set; }

    }
}
