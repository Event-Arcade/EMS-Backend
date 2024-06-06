using EMS.BACKEND.API.Enums;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface INotificationRepository
    {
        Task AddNotification(string title, string message, DatabaseChangeEventType eventType, string? userType, string? userId, EntityType entityType, int entityId, string? creatorId);
        Task<ICollection<Notification>> GetUnreadNotificationsByUserId(string userId);
        Task MarkNotificationAsRead(int notificationId);
        Task SendDatabaseChangeNotification(DatabaseChangeEventType eventType, EntityType entityType, string entityId, string? creatorId);
        Task SendDatabaseChangeNotification(DatabaseChangeEventType eventType, EntityType entityType, int entityId, string? creatorId);
        Task SendDatabaseChangeNotificationToUser(DatabaseChangeEventType eventType, EntityType entityType, int entityId, string userId);

    }
}
