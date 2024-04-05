using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStaticResourceController(IAdminStaticResourceRepository staticResourceRepository) : ControllerBase
    {
        [HttpPost("add"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddStaticResource(AdminStaticResource staticResource)
        {
            var result = await staticResourceRepository.CreateAsync(staticResource);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("delete/{staticResourceId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteStaticResource(string staticResourceId)
        {
            var result = await staticResourceRepository.DeleteAsync(staticResourceId);
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
        public async Task<IActionResult> GetAllStaticResources()
        {
            var result = await staticResourceRepository.FindAllAsync();
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("getbyid/{staticResourceId}")]
        public async Task<IActionResult> GetStaticResourceById(string staticResourceId)
        {
            var result = await staticResourceRepository.FindByIdAsync(staticResourceId);
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