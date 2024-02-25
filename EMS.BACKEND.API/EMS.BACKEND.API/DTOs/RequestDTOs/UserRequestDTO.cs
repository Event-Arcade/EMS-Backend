using System.ComponentModel.DataAnnotations;
namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class UserRequestDTO
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

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IFormFile? ProfilePicture { get; set; }
    }
}
