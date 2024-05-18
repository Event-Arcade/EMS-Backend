using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.Package;
using EMS.BACKEND.API.Extensions;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : Controller
    {
        private readonly IPackageRepository _packageRepository;

        public PackageController(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        [HttpGet("getall"), Authorize]
        public async Task<IActionResult> Get()
        {
            var response = await _packageRepository.FindAllAsync();
            if(response.Flag)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        
        [HttpGet("get/{id}"), Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _packageRepository.FindByIdAsync(id);
            if(response.Flag)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("create"), Authorize]
        public async Task<IActionResult> Create([FromForm] PackageRequestDTO packageRequestDTO)
        {
            var userId = User.GetUserId();
            var response = await _packageRepository.CreateAsync(userId, packageRequestDTO);
            if(response.Flag)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update/{id}"), Authorize]
        public async Task<IActionResult> Update(int id, [FromForm] PackageRequestDTO packageRequestDTO)
        {
            var userId = User.GetUserId();
            var response = await _packageRepository.UpdateAsync(userId, id, packageRequestDTO);
            if(response.Flag)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("delete/{id}"), Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId();
            var response = await _packageRepository.DeleteAsync(userId, id);
            if(response.Flag)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
