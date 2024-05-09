using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs.Shop;
using EMS.BACKEND.API.DTOs.ShopService;
using EMS.BACKEND.API.Extensions;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ShopServicesController(IShopServiceRepository serviceRepository) : Controller
    {
        [HttpGet("servicesbyshop")]
        public async Task<IActionResult> GetAllServices()
        {
            var response = await serviceRepository.FindAllAsync();
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("getservice/{id}"), Authorize]
        public async Task<IActionResult> GetService(int id)
        {

            var response = await serviceRepository.FindByIdAsync(id);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpDelete("delete/{id}"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var userId = User.GetUserId();
            var response = await serviceRepository.DeleteAsync(userId, id);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("create"), Authorize(Roles = "vendor"), RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> CreateService([FromForm] ShopServiceRequestDTO service)
        {
            var userId = User.GetUserId();
            var response = await serviceRepository.CreateAsync(userId,service);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("update/{serviceId}"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> UpdateService(int serviceId, [FromForm] ShopServiceRequestDTO service)
        {
            var userId = User.GetUserId();
            var response = await serviceRepository.UpdateAsync(userId,serviceId, service);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
