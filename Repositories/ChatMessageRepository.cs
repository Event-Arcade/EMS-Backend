using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class ChatMessageRepository(IServiceScopeFactory serviceScope) : IChatMessageRepository
    {
        

        public async Task<ChatMessage> AddMessage(string senderId, string receiverId, string message)
        {
            try{
                using(var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var chat = new ChatMessage
                    {
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Message = message,
                        Date = DateTime.Now
                    };
                    await _context.ChatMessages.AddAsync(chat);
                    await _context.SaveChangesAsync();
                    return chat;
                }
            }catch(Exception e){
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<ChatMessage>> GetMessage(string senderId, string receiverId)
        {
            try{
                using(var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await _context.ChatMessages
                        .Where(x => (x.SenderId == senderId && x.ReceiverId == receiverId) || (x.SenderId == receiverId && x.ReceiverId == senderId))
                        .OrderBy(x => x.Date)
                        .ToListAsync();
                }
            }catch(Exception e){
                throw new Exception(e.Message);
            }
        }
    }
}