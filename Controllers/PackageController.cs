using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController(IPackageRepository packageRepository) : Controller
    {
        [HttpGet("GetAllPackages/{userId}"),Authorize]
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

        [HttpGet("GetPackageById"),Authorize]
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
        public async Task<IActionResult> CreatePackage(PackageRequestDTO packageRequestDTO)
        {
            var response = await packageRepository.CreateAsync(packageRequestDTO);
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
        public async Task<IActionResult> UpdatePackage(PackageRequestDTO packageRequestDTO)
        {
            var result = await packageRepository.UpdateAsync(packageRequestDTO);
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
