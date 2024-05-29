using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.AdminStaticResource;
using EMS.BACKEND.API.Extensions;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStaticResourceController : ControllerBase
    {
        private readonly IAdminStaticResourceRepository _staticResourceRepository;

        public AdminStaticResourceController(IAdminStaticResourceRepository staticResourceRepository)
        {
            _staticResourceRepository = staticResourceRepository;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetStaticResources()
        {
            try
            {
                var staticResources = await _staticResourceRepository.FindAllAsync();
                return Ok(staticResources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetStaticResource(int id)
        {
            try
            {
                var staticResource = await _staticResourceRepository.FindByIdAsync(id);
                return Ok(staticResource);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create"), Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateStaticResource([FromForm] AdminStaticResourceRequestDTO staticResource)
        {
            try
            {
                var userId = User.GetUserId();
                var created = await _staticResourceRepository.CreateAsync(userId, staticResource);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateStaticResource(int id, [FromForm] AdminStaticResourceRequestDTO staticResource)
        {
            try
            {
                var userId = User.GetUserId();
                var updated = await _staticResourceRepository.UpdateAsync(userId, id, staticResource);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteStaticResource(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var deleted = await _staticResourceRepository.DeleteAsync(userId, id);
                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}