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
            var staticResources = await _staticResourceRepository.FindAllAsync();
            if (staticResources.Flag)
            {
                return Ok(staticResources);
            }
            return BadRequest(staticResources);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetStaticResource(int id)
        {
            var staticResource = await _staticResourceRepository.FindByIdAsync(id);
            if (staticResource.Flag)
            {
                return Ok(staticResource);
            }
            return BadRequest(staticResource);
        }

        [HttpPost("create"), Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateStaticResource([FromForm] AdminStaticResourceRequestDTO staticResource)
        {
            var userId = User.GetUserId();
            var created = await _staticResourceRepository.CreateAsync(userId, staticResource);
            if (created.Flag)
            {
                return Ok(created);
            }

            return BadRequest(created);
        }

        [HttpPut("update/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateStaticResource(int id, [FromForm] AdminStaticResourceRequestDTO staticResource)
        {
            var userId = User.GetUserId();
            var updated = await _staticResourceRepository.UpdateAsync(userId, id, staticResource);
            if (updated.Flag)
            {
                return Ok(updated);
            }

            return BadRequest(updated);
        }

        [HttpDelete("delete/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteStaticResource(int id)
        {
            var userId = User.GetUserId();
            var deleted = await _staticResourceRepository.DeleteAsync(userId, id);
            if (deleted.Flag)
            {
                return Ok(deleted);
            }

            return BadRequest(deleted);
        }
    }
}