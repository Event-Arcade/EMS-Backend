using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.Chat;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly IServiceScopeFactory serviceScope;
        private readonly ICloudProviderRepository _cloudProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatMessageRepository(IServiceScopeFactory serviceScope, ICloudProviderRepository cloudProviderRepository, UserManager<ApplicationUser> userManager)
        {
            this._cloudProvider = cloudProviderRepository;
            this.serviceScope = serviceScope;
            this._userManager = userManager;
        }
        public async Task<ChatMessage> AddMessage(string senderId, string receiverId, string message)
        {
            try
            {
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var chat = new ChatMessage
                    {
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Message = message,
                        Date = DateTime.Now,
                        IsRead = false
                    };
                    var result = await _context.ChatMessages.AddAsync(chat);
                    await _context.SaveChangesAsync();

                    return result.Entity;

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<ChatMessage> GetMessageById(int messageId)
        {
            try
            {
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await _context.ChatMessages.FirstOrDefaultAsync(x => x.Id == messageId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task SetUserActive(string userId, string connectionId)
        {
            try
            {
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    if (user != null)
                    {
                        user.OnlineId = connectionId;
                        user.IsActive = true;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("User not found");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string?> NotifyUserOffline(string userId)
        {
            try
            {
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.OnlineId == userId);
                    if (user != null)
                    {
                        user.OnlineId = null;
                        user.IsActive = false;
                        await _context.SaveChangesAsync();
                        return user.Id;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }
        }

        public async Task<string> GetUserConnectionId(string userId)
        {
            try
            {
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    return user.OnlineId;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<ChatInboxDTO>> GetMyChatUsers(string userId)
        {
            try
            {
                using (var scope = serviceScope.CreateScope())
                {
                    // get each user that has chatted with the current user 
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var users = await _context.ChatMessages
                        .Where(x => x.ReceiverId == userId || x.SenderId == userId)
                        .Select(x => x.ReceiverId == userId ? x.SenderId : x.ReceiverId)
                        .Distinct()
                        .ToListAsync();

                    var chatUsers = new List<ChatInboxDTO>();
                    foreach (var user in users)
                    {
                        var chatUser = new ChatInboxDTO
                        {
                            Id = user,
                            FirstName = (await _userManager.FindByIdAsync(user)).FirstName,
                            LastMessage = (await _context.ChatMessages
                                .Where(x => (x.ReceiverId == userId && x.SenderId == user) || (x.ReceiverId == user && x.SenderId == userId))
                                .OrderByDescending(x => x.Date)
                                .FirstOrDefaultAsync()).Message,
                            LastMessageDate = (await _context.ChatMessages
                                .Where(x => (x.ReceiverId == userId && x.SenderId == user) || (x.ReceiverId == user && x.SenderId == userId))
                                .OrderByDescending(x => x.Date)
                                .FirstOrDefaultAsync()).Date,
                            IsRead = (await _context.ChatMessages
                                .Where(x => x.ReceiverId == userId && x.SenderId == user && x.IsRead == false)
                                .ToListAsync()).Count == 0,
                            IsActive = (await _context.Users.FirstOrDefaultAsync(x => x.Id == user)).IsActive,
                            UnreadMessages = (await _context.ChatMessages
                                .Where(x => x.ReceiverId == userId && x.SenderId == user && x.IsRead == false)
                                .ToListAsync()).Count
                        };
                        chatUsers.Add(chatUser);
                    }

                    // convert profile photos to urls
                    foreach (var chatUser in chatUsers)
                    {
                        var user = await _userManager.FindByIdAsync(chatUser.Id);
                        chatUser.ProfilePictureURL = _cloudProvider.GeneratePreSignedUrlForDownload(user.ProfilePicturePath);

                    }

                    return chatUsers;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ChatInboxDTO> SetMessageRead(string userId, string senderId)
        {
            try
            {
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var messages = await _context.ChatMessages
                        .Where(x => x.ReceiverId == userId && x.SenderId == senderId && x.IsRead == false)
                        .ToListAsync();
                    foreach (var message in messages)
                    {
                        message.IsRead = true;
                    }
                    await _context.SaveChangesAsync();
                    return new ChatInboxDTO
                    {
                        Id = senderId,
                        FirstName = (await _context.Users.FirstOrDefaultAsync(x => x.Id == senderId)).FirstName,
                        ProfilePictureURL = _cloudProvider.GeneratePreSignedUrlForDownload((await _context.Users.FirstOrDefaultAsync(x => x.Id == senderId)).ProfilePicturePath),
                        UnreadMessages = 0,
                        LastMessage = (await _context.ChatMessages
                            .Where(x => (x.ReceiverId == userId && x.SenderId == senderId) || (x.ReceiverId == senderId && x.SenderId == userId))
                            .OrderByDescending(x => x.Date)
                            .FirstOrDefaultAsync()).Message,
                        LastMessageDate = (await _context.ChatMessages
                            .Where(x => (x.ReceiverId == userId && x.SenderId == senderId) || (x.ReceiverId == senderId && x.SenderId == userId))
                            .OrderByDescending(x => x.Date)
                            .FirstOrDefaultAsync()).Date,
                        IsRead = true,
                        IsActive = (await _context.Users.FirstOrDefaultAsync(x => x.Id == senderId)).IsActive

                    };
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<ChatMessageDTO>> GetChatHistory(string senderId, string receiverId)
        {
            try{
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var messages = await _context.ChatMessages
                        .Where(x => (x.ReceiverId == receiverId && x.SenderId == senderId) || (x.ReceiverId == senderId && x.SenderId == receiverId))
                        .OrderBy(x => x.Date)
                        .ToListAsync();
                    var chatMessages = new List<ChatMessageDTO>();
                    foreach (var message in messages)
                    {
                        var chatMessage = new ChatMessageDTO
                        {
                            Id = message.Id,
                            SenderId = message.SenderId,
                            ReceiverId = message.ReceiverId,
                            Message = message.Message,
                            Date = message.Date,
                            IsRead = message.IsRead
                        };
                        chatMessages.Add(chatMessage);
                    }
                    return chatMessages;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            
            }
        }

        public async Task<ChatInboxDTO> GetMyChatUser(string userId, string receiverId)
        {
            try{
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var chatUser = new ChatInboxDTO
                    {
                        Id = receiverId,
                        FirstName = (await _context.Users.FirstOrDefaultAsync(x => x.Id == receiverId)).FirstName,
                        ProfilePictureURL = _cloudProvider.GeneratePreSignedUrlForDownload((await _context.Users.FirstOrDefaultAsync(x => x.Id == receiverId)).ProfilePicturePath),
                        LastMessage = (await _context.ChatMessages
                            .Where(x => (x.ReceiverId == userId && x.SenderId == receiverId) || (x.ReceiverId == receiverId && x.SenderId == userId))
                            .OrderByDescending(x => x.Date)
                            .FirstOrDefaultAsync()).Message,
                        LastMessageDate = (await _context.ChatMessages
                            .Where(x => (x.ReceiverId == userId && x.SenderId == receiverId) || (x.ReceiverId == receiverId && x.SenderId == userId))
                            .OrderByDescending(x => x.Date)
                            .FirstOrDefaultAsync()).Date,
                        IsRead = (await _context.ChatMessages
                            .Where(x => x.ReceiverId == userId && x.SenderId == receiverId && x.IsRead == false)
                            .ToListAsync()).Count == 0,
                        IsActive = (await _context.Users.FirstOrDefaultAsync(x => x.Id == receiverId)).IsActive,
                        UnreadMessages = (await _context.ChatMessages
                            .Where(x => x.ReceiverId == userId && x.SenderId == receiverId && x.IsRead == false)
                            .ToListAsync()).Count
                    };
                    return chatUser;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ChatInboxDTO> GetNewChatUser(string userId, string receiverId)
        {
            try{
                using (var scope = serviceScope.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var chatUser = new ChatInboxDTO
                    {
                        Id = receiverId,
                        FirstName = (await _context.Users.FirstOrDefaultAsync(x => x.Id == receiverId)).FirstName,
                        ProfilePictureURL = _cloudProvider.GeneratePreSignedUrlForDownload((await _context.Users.FirstOrDefaultAsync(x => x.Id == receiverId)).ProfilePicturePath),
                        IsActive = (await _context.Users.FirstOrDefaultAsync(x => x.Id == receiverId)).IsActive,
                    
                    };
                    return chatUser;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
