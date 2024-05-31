using Microsoft.AspNetCore.SignalR;
using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace EMS.BACKEND.API.Hubs
{
    public class PersonalChatHub : Hub
    {
        private readonly IChatMessageRepository _chatMessageRepository;

        public PersonalChatHub(IChatMessageRepository chatMessageRepository)
        {
            _chatMessageRepository = chatMessageRepository;
        }

        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = await _chatMessageRepository.NotifyUserOffline(Context.ConnectionId);
            await Clients.AllExcept(Context.ConnectionId).SendAsync("UserOffline", userId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SetActive(string userId)
        {
            await _chatMessageRepository.SetUserActive(userId, Context.ConnectionId);
            await Clients.AllExcept(Context.ConnectionId).SendAsync("UserConnected", userId);
        }

        public async Task NotifyUserOffline()
        {
            var userId = await _chatMessageRepository.NotifyUserOffline(Context.ConnectionId);
            await Clients.AllExcept(Context.ConnectionId).SendAsync("UserOffline", userId);
        }

    }
}
