using System.ComponentModel.DataAnnotations;

namespace EMS.BACKEND.API.DTOs.Account
{
    public class LoginDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}