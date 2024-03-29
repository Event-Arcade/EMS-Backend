using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class ServiceRequestDTO
    {
        public string Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public double Price { get; set; }
        [Required, NotNull] public string CategoryId { get; set; }
        [Required, NotNull] public string ShopId { get; set; }
    }
}