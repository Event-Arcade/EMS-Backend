using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController(IPackageRepository packageRepository) : Controller
    {
        [HttpGet("GetAllPackages")]
        public async Task<IActionResult> GetAllPackages()
        {
            var packages = await packageRepository.GetAllPackages();
            return Ok(packages);
        }

        [HttpGet("GetPackageById")]
        public async Task<IActionResult> GetPackageById(string id)
        {
            var package = await packageRepository.GetPackageById(id);
            return Ok(package);
        }

        [HttpPost("CreatePackage")]
        public async Task<IActionResult> CreatePackage(PackageRequestDTO packageRequestDTO)
        {
            var response = await packageRepository.CreatePackage(packageRequestDTO);
            return Ok(response);
        }

        [HttpPut("UpdatePackage")]
        public async Task<IActionResult> UpdatePackage(PackageRequestDTO packageRequestDTO)
        {
            var response = await packageRepository.UpdatePackage(packageRequestDTO);
            return Ok(response);
        }

        [HttpDelete("DeletePackage")]
        public async Task<IActionResult> DeletePackage(string id)
        {
            var response = await packageRepository.DeletePackage(id);
            return Ok(response);
        }
    }
}
