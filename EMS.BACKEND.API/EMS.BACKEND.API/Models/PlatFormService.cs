

namespace EMS.BACKEND.API.Models
{
    public class PlatFormService
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Double Price { get; set; } = 0;
        public Double SuppliedAreaRange { get; set; } = 0;
        public int MaxHeadCount { get; set; } = 0;
        public Double Longitude { get; set; } = 0;
        public Double Latitude { get; set; } = 0;
        public string ContactNumber { get; set; } = string.Empty;
    }
}
