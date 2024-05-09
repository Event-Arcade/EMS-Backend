namespace EMS.BACKEND.API.DTOs.Account
{
    public class UpdateUserDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Province { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}