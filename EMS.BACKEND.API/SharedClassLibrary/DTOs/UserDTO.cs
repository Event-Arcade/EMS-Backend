using System.ComponentModel.DataAnnotations;
namespace SharedClassLibrary.DTOs
{
    public class UserDTO
    {
        public string? Id { get; set; } = string.Empty;
        [Required] public string FirstName { get; set; } = string.Empty; 
        [Required] public string LastName { get; set; } = string.Empty;
        [Required] public string Street { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;
        [Required] public string PostalCode { get; set; } = string.Empty;
        [Required] public string Province { get; set; } = string.Empty;
        [Required] public Double Longitude { get; set; }
        [Required] public Double Latitude { get; set; }

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
    }
}
