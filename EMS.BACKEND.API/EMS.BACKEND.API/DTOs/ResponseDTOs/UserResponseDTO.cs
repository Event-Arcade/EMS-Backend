using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public class UserResponseDTO
    {
        public string? Id { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Street { get; set; }
        [Required] public string City { get; set; }
        [Required] public string PostalCode { get; set; }
        [Required] public string Province { get; set; }
        [Required] public double Longitude { get; set; }
        [Required] public double Latitude { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

    }
}
