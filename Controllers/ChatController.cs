using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.Chat;
using EMS.BACKEND.API.Extensions;
using EMS.BACKEND.API.Hubs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<EMSHub> _hubContext;

        public ChatController(IChatMessageRepository chatMessageRepository, UserManager<ApplicationUser> userManager, IHubContext<Hubs.EMSHub> hubContext)
        {
            _chatMessageRepository = chatMessageRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpPost("send"), Authorize]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDTO chatMessageDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.GetUserId();
                var sender = await _userManager.FindByIdAsync(userId);
                var receiver = await _userManager.FindByIdAsync(chatMessageDTO.ReceiverId);

                if (sender == null || receiver == null)
                {
                    return BadRequest("Invalid sender or receiver");
                }

                var result = await _chatMessageRepository.AddMessage(userId, chatMessageDTO.ReceiverId, chatMessageDTO.Message);

                if (result != null)
                {
                    // check if receiver is online
                    var receiverConnectionId = await _chatMessageRepository.GetUserConnectionId(chatMessageDTO.ReceiverId);
                    if (!string.IsNullOrEmpty(receiverConnectionId))
                    {
                        await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", result);
                    }
                    // TODO: send email notification if receiver is offline
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Failed to send message");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-my-chat/{recieverId}"), Authorize]
        public async Task<IActionResult> GetChatById(string recieverId)
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _chatMessageRepository.GetMyChatUser(userId, recieverId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-chat-history/{recieverId}"), Authorize]
        public async Task<IActionResult> GetChatHistory(string recieverId)
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _chatMessageRepository.GetChatHistory(userId, recieverId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("set-message-read/{recieverId}"), Authorize]
        public async Task<IActionResult> SetMessageRead(string recieverId)
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _chatMessageRepository.SetMessageRead(userId, recieverId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("get-my-chats"), Authorize]
        public async Task<IActionResult> GetMyChatUsers()
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _chatMessageRepository.GetMyChatUsers(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-new-chat/{id}"), Authorize]
        public async Task<IActionResult> GetNewChat(string id)
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _chatMessageRepository.GetNewChatUser(userId, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}