using EMS.BACKEND.API.Models;

namespace  EMS.BACKEND.API.Contracts{
    public interface IChatMessageRepository
    {
        Task<ChatMessage> AddMessage(string senderId, string receiverId, string message);
        Task<IEnumerable<ChatMessage>> GetMessage(string senderId, string receiverId);
    }
}

    
