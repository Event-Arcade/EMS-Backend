using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.Account
{
    public class RegisterUserDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}