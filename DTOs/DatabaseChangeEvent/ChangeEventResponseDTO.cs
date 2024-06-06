using EMS.BACKEND.API.Enums;

namespace EMS.BACKEND.API.DTOs
{
    public class ChangeEventResponseDTO
    {
        public DatabaseChangeEventType EventType { get; set; }
        public EntityType EntityType { get; set; }
        public string EntityId { get; set; }
        public ChangeEventResponseDTO(DatabaseChangeEventType eventType, EntityType entityType, string entityId)
        {
            EventType = eventType;
            EntityType = entityType;
            EntityId = entityId;
        }
        public ChangeEventResponseDTO(DatabaseChangeEventType eventType, EntityType entityType, int entityId)
        {
            EventType = eventType;
            EntityType = entityType;
            EntityId = entityId.ToString();
        }
    }
}