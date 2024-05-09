namespace EMS.BACKEND.API.Models
{
    public class FeedBackStaticResource : BaseStaticResource
    {
        public int FeedBackId { get; set; }
        public FeedBack FeedBack { get; set; }
    }
}