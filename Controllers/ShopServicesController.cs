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
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllServices()
        {
            try
            {
                var response = await serviceRepository.FindAllAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getservice/{id}"), Authorize]
        public async Task<IActionResult> GetService(int id)
        {

            try
            {
                var response = await serviceRepository.FindByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var response = await serviceRepository.DeleteAsync(userId, id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create"), Authorize(Roles = "vendor"), RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> CreateService([FromForm] ShopServiceRequestDTO service)
        {
            try
            {
                var userId = User.GetUserId();
                var response = await serviceRepository.CreateAsync(userId, service);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{serviceId}"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> UpdateService(int serviceId, [FromForm] ShopServiceRequestDTO service)
        {
            try
            {
                var userId = User.GetUserId();
                var response = await serviceRepository.UpdateAsync(userId, serviceId, service);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
