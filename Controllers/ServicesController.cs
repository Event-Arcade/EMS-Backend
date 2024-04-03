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
        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices()
        {
            //var response = await serviceRepository.GetAllServices();
            return Ok();
        }

        [HttpGet("myservices{id}"), Authorize]
        public async Task<IActionResult> GetService(string id)
        {
           // var response = await serviceRepository.GetServiceById(id);
            return Ok();
        }

        [HttpDelete("delete{id}"), Authorize]
        public async Task<IActionResult> DeleteService(string id)
        {
            //var response = await serviceRepository.Delete(id);
            return Ok();
        }

        [HttpPost("create"), Authorize]
        public async Task<IActionResult> CreateService(ServiceRequestDTO serviceRequestDTO)
        {
            //var response = await serviceRepository.Create(serviceRequestDTO);
            return Ok();
        }

        [HttpPut("update"), Authorize]
        public async Task<IActionResult> UpdateService(ServiceRequestDTO serviceRequestDTO)
        {
            //var response = await serviceRepository.Update(serviceRequestDTO);
            return Ok();
        }
    }
}
