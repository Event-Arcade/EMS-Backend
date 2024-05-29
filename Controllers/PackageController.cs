using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.Package;
using EMS.BACKEND.API.DTOs.SubPackage;
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
            try
            {
                var response = await _packageRepository.FindAllAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get/{id}"), Authorize]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _packageRepository.FindByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create"), Authorize]
        public async Task<IActionResult> Create([FromBody] PackageRequestDTO packageRequestDTO)
        {
            try
            {
                var userId = User.GetUserId();
                var response = await _packageRepository.CreateAsync(userId, packageRequestDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}"), Authorize]
        public async Task<IActionResult> Update(int id, [FromForm] PackageRequestDTO packageRequestDTO)
        {
            try
            {
                var userId = User.GetUserId();
                var response = await _packageRepository.UpdateAsync(userId, id, packageRequestDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}"), Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var response = await _packageRepository.DeleteAsync(userId, id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-sub-package/{id}"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> UpdateSubPackage(int id, [FromForm] SubPackageRequestDTO subPackageRequestDTO)
        {
            try
            {
                var userId = User.GetUserId();
                var response = await _packageRepository.UpdateSubPackage(userId, id, subPackageRequestDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("get-sub-packages"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> GetSubPackages()
        {
            try
            {
                var userId = User.GetUserId();
                var response = await _packageRepository.GetSubPackages(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
