using Contracts;
using EMS.BACKEND.API.DTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Extensions;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackController(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        [HttpPost("create"), Authorize]
        public async Task<IActionResult> AddFeedBack([FromForm] FeedBackRequestDTO feedBackRequestDTO)
        {
            try
            {
                var userId = User.GetUserId();

                var result = await _feedbackRepository.CreateAsync(userId, feedBackRequestDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{feedBackId}"), Authorize]
        public async Task<IActionResult> DeleteFeedBack(int feedBackId)
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _feedbackRepository.DeleteAsync(userId, feedBackId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllFeedBacks()
        {
            try
            {
                var result = await _feedbackRepository.FindAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}