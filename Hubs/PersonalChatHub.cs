using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EMS.BACKEND.API.Hubs
{
    [Authorize]
    public class PersonalChatHub(IChatMessageRepository chatMessageRepository) : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            try
            {
                var chat = await chatMessageRepository.AddMessage(senderId, receiverId, message);
                await Clients.All.SendAsync("ReceiveMessage", chat);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task GetMessages(string senderId, string receiverId)
        {
            try
            {
                var messages = await chatMessageRepository.GetMessage(senderId, receiverId);
                await Clients.Caller.SendAsync("ReceiveMessages", messages);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public override async Task OnConnectedAsync()
        {
            //send a message to the client
            await Clients.Caller.SendAsync("ReceiveMessage", "Welcome to the chat room");

            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

    }
}