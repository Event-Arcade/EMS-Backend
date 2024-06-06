namespace EMS.BACKEND.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EMS.BACKEND.API.Contracts;
    using EMS.BACKEND.API.DTOs;
    using EMS.BACKEND.API.Extensions;
    using EMS.BACKEND.API.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet("unread/"), Authorize]
        public async Task<IActionResult> GetUnreadNotificationsByUserId()
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _notificationRepository.GetUnreadNotificationsByUserId(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("mark-as-read/{notificationId}"), Authorize]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            try
            {
                await _notificationRepository.MarkNotificationAsRead(notificationId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}