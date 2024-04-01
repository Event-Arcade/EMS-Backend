namespace EMS.BACKEND.API.Models
{
    public class Package
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
