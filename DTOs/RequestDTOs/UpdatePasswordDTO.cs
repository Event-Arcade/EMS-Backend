namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class UpdatePasswordDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}