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

        // [HttpGet("GetFeedBacksByShopId")]
        // public async Task<BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>> GetFeedBacksByShopId(string shopId)
        // {
        //     return await _feedbackRepository.GetFeedBacksByShopId(shopId);
        // }

        // [HttpGet("GetFeedBacksByServiceId")]
        // public async Task<BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>> GetFeedBacksByServiceId(string serviceId)
        // {
        //     var result = await _feedbackRepository.GetFeedBacksByServiceId(serviceId);
        //     if (result.Flag)
        //     {
        //         return Ok(result);
        //     }
        //     else
        //     {
        //         return BadRequest(result);
        //     }
        // }

        [HttpPost("create"), Authorize]
        public async Task<IActionResult> AddFeedBack([FromForm]FeedBackRequestDTO feedBackRequestDTO)
        {
            var userId = User.GetUserId(); 

            var result = await _feedbackRepository.CreateAsync(userId, feedBackRequestDTO);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        // TODO: Try to do implement otherwise delete
        // [HttpPut("UpdateFeedBack")]
        // public async Task<BaseResponseDTO<FeedBackResponseDTO>> UpdateFeedBack(FeedBackRequestDTO feedBackRequestDTO)
        // {
        //     var result = await _feedbackRepository.Update(feedBackRequestDTO);
        //     if (result.Flag)
        //     {
        //         return Ok(result);
        //     }
        //     else
        //     {
        //         return BadRequest(result);
        //     }
        // }

        [HttpDelete("delete/{feedBackId}"), Authorize]
        public async Task<IActionResult> DeleteFeedBack(int feedBackId)
        {
            var userId = User.GetUserId();
            var result = await _feedbackRepository.DeleteAsync(userId, feedBackId);
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
        public async Task<IActionResult> GetAllFeedBacks()
        {
            var result = await _feedbackRepository.FindAllAsync();
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