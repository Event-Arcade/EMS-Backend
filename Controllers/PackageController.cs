using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController(IPackageRepository packageRepository) : Controller
    {
        [HttpGet("GetAllPackages/{userId}"), Authorize]
        public async Task<IActionResult> GetAllPackages(string userId)
        {
            var result = await packageRepository.GetAllPackagesByUser(userId);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("GetPackageById"), Authorize]
        public async Task<IActionResult> GetPackageById(string id)
        {
            var result = await packageRepository.FindByIdAsync(id);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("CreatePackage"), Authorize(Roles = "Client")]
        public async Task<IActionResult> CreatePackage(Package package)
        {
            var response = await packageRepository.CreateAsync(package);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("UpdatePackage"), Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdatePackage([FromQuery] String packageId, [FromForm] Package package)
        {
            var result = await packageRepository.UpdateAsync(packageId, package);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("DeletePackage/{id}"), Authorize(Roles = "Client")]
        public async Task<IActionResult> DeletePackage(string id)
        {
            var response = await packageRepository.DeleteAsync(id);
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
