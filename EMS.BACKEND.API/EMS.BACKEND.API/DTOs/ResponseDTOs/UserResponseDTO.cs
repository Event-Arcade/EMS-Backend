using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public class UserResponseDTO
    {
        public string? Id { get; set; } = string.Empty;
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required] public string Street { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;
        [Required] public string PostalCode { get; set; } = string.Empty;
        [Required] public string Province { get; set; } = string.Empty;
        [Required] public double Longitude { get; set; }
        [Required] public double Latitude { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

    }
}
