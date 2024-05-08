using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.Models
{
    public class SubPackage
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int PackageId { get; set; }
        public Package Package { get; set; }
        public PackageStatus Status { get; set; }
        public int ServiceId { get; set; }
        public ShopService Service { get; set; }

    }
}
