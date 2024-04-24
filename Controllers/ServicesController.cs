using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController(IServiceRepository serviceRepository) : Controller
    {
        [HttpGet("servicesbyshop/{shopId}")]
        public async Task<IActionResult> GetAllServices(string shopId)
        {
            var response = await serviceRepository.GetServicesByShopId(shopId);
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
        public async Task<IActionResult> GetService(string id)
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

        [HttpDelete("delete/{id}"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteService(string id)
        {
            var response = await serviceRepository.DeleteAsync(id);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("create"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateService(Service service)
        {
            var response = await serviceRepository.CreateAsync(service);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("update"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateService([FromQuery] String serviceId, [FromForm] Service service)
        {
            var response = await serviceRepository.UpdateAsync(serviceId, service);
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
