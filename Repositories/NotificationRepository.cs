namespace EMS.BACKEND.API.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EMS.BACKEND.API.Contracts;
    using EMS.BACKEND.API.DbContext;
    using EMS.BACKEND.API.DTOs;
    using EMS.BACKEND.API.Enums;
    using EMS.BACKEND.API.Hubs;
    using EMS.BACKEND.API.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    public class NotificationRepository : INotificationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceScopeFactory _serviceScope;
        private readonly IHubContext<EMSHub> _hubContext;

        public NotificationRepository(IServiceScopeFactory serviceScopeFactory, UserManager<ApplicationUser> userManager, IHubContext<EMSHub> hubContext)
        {
            _userManager = userManager;
            _serviceScope = serviceScopeFactory;
            _hubContext = hubContext;
        }

        public async Task AddNotification(string title, string message, DatabaseChangeEventType eventType, string? userType, string? userId, EntityType entityType, int entityId, string? creatorId)
        {
            try
            {
                using (var scope = _serviceScope.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    if (userType == "admin")
                    {
                        var users = await _userManager.GetUsersInRoleAsync("admin");
                        foreach (var user in users)
                        {
                            if (user.Id == creatorId)
                            {
                                continue;
                            }
                            var notification = new Notification
                            {
                                Title = title,
                                Message = message,
                                UserId = user.Id,
                                IsRead = false,
                                CreatedAt = DateTime.Now,
                                EventType = eventType,
                                EntityId = entityId,
                                EntityType = entityType
                            };
                            var response = await dbContext.Notifications.AddAsync(notification);
                            await dbContext.SaveChangesAsync();
                            // check if user is online and send notification
                            if (user.IsActive)
                            {
                                await _hubContext.Clients.Client(user.OnlineId).SendAsync("ReceiveNotification", response.Entity);
                            }
                        }
                        return;
                    }
                    else if (userType == "vendor")
                    {
                        var users = await _userManager.GetUsersInRoleAsync("vendor");
                        foreach (var user in users)
                        {
                            if (user.Id == creatorId)
                            {
                                continue;
                            }
                            var notification = new Notification
                            {
                                Title = title,
                                Message = message,
                                UserId = user.Id,
                                IsRead = false,
                                CreatedAt = DateTime.Now,
                                EntityId = entityId,
                                EntityType = entityType,
                                EventType = eventType
                            };
                            var response = await dbContext.Notifications.AddAsync(notification);
                            await dbContext.SaveChangesAsync();
                            // check if user is online and send notification
                            if (user.IsActive)
                            {
                                await _hubContext.Clients.Client(user.OnlineId).SendAsync("ReceiveNotification", response.Entity);
                            }
                        }
                        return;
                    }
                    else if (userType == "client")
                    {
                        var users = await _userManager.GetUsersInRoleAsync("client");
                        foreach (var user in users)
                        {
                            if (user.Id == creatorId)
                            {
                                continue;
                            }
                            var notification = new Notification
                            {
                                Title = title,
                                Message = message,
                                UserId = user.Id,
                                IsRead = false,
                                CreatedAt = DateTime.Now,
                                EntityId = entityId,
                                EntityType = entityType,
                                EventType = eventType
                            };
                            var response = await dbContext.Notifications.AddAsync(notification);
                            await dbContext.SaveChangesAsync();
                            // check if user is online and send notification
                            if (user.IsActive)
                            {
                                await _hubContext.Clients.Client(user.OnlineId).SendAsync("ReceiveNotification", response.Entity);
                            }
                        }
                        return;
                    }
                    else
                    {
                        var notification = new Notification
                        {
                            Title = title,
                            Message = message,
                            UserId = userId,
                            IsRead = false,
                            CreatedAt = DateTime.Now,
                            EntityId = entityId,
                            EntityType = entityType,
                            EventType = eventType
                        };
                        var response = await dbContext.Notifications.AddAsync(notification);
                        await dbContext.SaveChangesAsync();
                        // check if user is online and send notification
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user.IsActive)
                        {
                            await _hubContext.Clients.Client(user.OnlineId).SendAsync("ReceiveNotification", response.Entity);
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<Notification>> GetUnreadNotificationsByUserId(string userId)
        {
            try
            {
                using (var scope = _serviceScope.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var notifications = await dbContext.Notifications.Where(x => x.UserId == userId && x.IsRead == false).ToListAsync();
                    return notifications;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task MarkNotificationAsRead(int notificationId)
        {
            try
            {
                using (var scope = _serviceScope.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // remove notification 
                    var notification = await dbContext.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId);
                    if (notification != null)
                    {
                        dbContext.Notifications.Remove(notification);
                        await dbContext.SaveChangesAsync();
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendDatabaseChangeNotification(DatabaseChangeEventType eventType, EntityType entityType, string entityId, string? creatorId)
        {
            try
            {
                // send all users but except the creator
                using (var scope = _serviceScope.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var users = await dbContext.Users.ToListAsync();
                    foreach (var user in users)
                    {
                        if (user.Id == creatorId)
                        {
                            continue;
                        }
                        // check if user is online and send notification
                        if (user.IsActive)
                        {
                            await _hubContext.Clients.Client(user.OnlineId).SendAsync("ReceiveDatabaseChangeEvent", new ChangeEventResponseDTO(eventType, entityType, entityId));
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendDatabaseChangeNotification(DatabaseChangeEventType eventType, EntityType entityType, int entityId, string? creatorId)
        {
            try
            {
                // send all users but except the creator
                using (var scope = _serviceScope.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var users = await dbContext.Users.ToListAsync();
                    foreach (var user in users)
                    {
                        if (user.Id == creatorId)
                        {
                            continue;
                        }
                        // check if user is online and send notification
                        if (user.IsActive)
                        {
                            await _hubContext.Clients.Client(user.OnlineId).SendAsync("ReceiveDatabaseChangeEvent", new ChangeEventResponseDTO(eventType, entityType, entityId));
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendDatabaseChangeNotificationToUser(DatabaseChangeEventType eventType, EntityType entityType, int entityId, string userId)
        {
            try{
                using (var scope = _serviceScope.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    if (user != null)
                    {
                        // check if user is online and send notification
                        if (user.IsActive)
                        {
                            await _hubContext.Clients.Client(user.OnlineId).SendAsync("ReceiveDatabaseChangeEvent", new ChangeEventResponseDTO(eventType, entityType, entityId));
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}