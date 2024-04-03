namespace EMS.BACKEND.API.Models
{
    public class Service
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
        public double Rating { get; set; }
        public string? ShopId { get; set; }
        public virtual Category? Category { get; set; }
        public List<string>? staticResourcesPaths { get; set; }
        public virtual List<FeedBack>? FeedBacks { get; set; }
    }
}
