using System.ComponentModel.DataAnnotations.Schema;

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
        public string? CategoryId { get; set; }
        public virtual ICollection<ServiceStaticResources>? StaticResourcesPaths { get; set; }
        public virtual ICollection<FeedBack>? FeedBacks { get; set; }
        [NotMapped]
        public ICollection<IFormFile> formFiles { get; set; }
    }
}
