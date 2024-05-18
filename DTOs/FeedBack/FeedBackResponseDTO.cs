namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public class FeedBackResponseDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime PostedOn { get; set; }
        public double Rating { get; set; }
        public int ServiceId { get; set; }
        public string ApplicationUserId { get; set; }
        public ICollection<string> FeedBackStaticResourcesUrls { get; set; }
    }
}
