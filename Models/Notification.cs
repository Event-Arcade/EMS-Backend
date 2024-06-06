
using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.Models
{
    public class Notification
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public EntityType EntityType { get; set; }
        public int EntityId { get; set; }
        public DatabaseChangeEventType EventType { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}