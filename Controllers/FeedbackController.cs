using Contracts;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController(IFeedbackRepository feedbackRepository) : ControllerBase
    {
        [HttpPost("add"),Authorize]
        public async Task<IActionResult> AddFeedback(FeedBack feedback)
        {
            var result = await feedbackRepository.CreateAsync(feedback);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("delete/{feedbackId}"),Authorize]
        public async Task<IActionResult> DeleteFeedback(string feedbackId)
        {
            var result = await feedbackRepository.DeleteAsync(feedbackId);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var result = await feedbackRepository.FindAllAsync();
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("getbyid/{feedbackId}")]
        public async Task<IActionResult> GetFeedbackById(string feedbackId)
        {
            var result = await feedbackRepository.FindByIdAsync(feedbackId);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPut("update"),Authorize]
        public async Task<IActionResult> UpdateFeedback([FromQuery] String feedbackid ,[FromForm]FeedBack feedback)
        {
            var result = await feedbackRepository.UpdateAsync(feedbackid,feedback);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("getbyserviceid/{serviceId}")]
        public async Task<IActionResult> GetFeedbackByServiceId(string serviceId)
        {
            var result = await feedbackRepository.GetFeedBacksByServiceId(serviceId);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("getbyshopid/{shopId}")]
        public async Task<IActionResult> GetFeedbackByShopId(string shopId)
        {
            var result = await feedbackRepository.GetFeedBacksByShopId(shopId);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}