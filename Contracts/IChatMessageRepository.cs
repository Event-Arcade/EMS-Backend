using EMS.BACKEND.API.DTOs.Chat;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IChatMessageRepository
    {
        Task<ChatMessage> AddMessage(string senderId, string receiverId, string message);
        Task<ChatMessage> GetMessageById(int messageId);
        Task<ChatInboxDTO> SetMessageRead(string userId, string senderId);
        Task SetUserActive(string userId, string connectionId);
        Task<string> NotifyUserOffline(string userId);
        Task<string> GetUserConnectionId(string userId);
        Task<IEnumerable<ChatInboxDTO>> GetMyChatUsers(string userId);
        Task<IEnumerable<ChatMessageDTO>> GetChatHistory(string senderId, string receiverId);
        Task<ChatInboxDTO> GetMyChatUser(string userId, string receiverId);
        Task<ChatInboxDTO> GetNewChatUser(string userId, string receiverId);
    }
}


