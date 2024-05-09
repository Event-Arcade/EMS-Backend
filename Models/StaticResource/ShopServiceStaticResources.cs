namespace EMS.BACKEND.API.Models
{
    public class ShopServiceStaticResources : BaseStaticResource
    {
        public int ServiceId { get; set; }
        public ShopService Service { get; set; }
    }
}