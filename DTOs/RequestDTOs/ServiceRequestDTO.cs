using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.DTOs.RequestDTOs
{
    public class ServiceRequestDTO 
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
        public IEnumerable<IFormFile>? StaticResources { get; set; }
        public virtual List<FeedBack>? FeedBacks { get; set; }
        [Required, NotNull] public string CategoryId { get; set; }
        [Required, NotNull] public string ShopId { get; set; }
    }
}