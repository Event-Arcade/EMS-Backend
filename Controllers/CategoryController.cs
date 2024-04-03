using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryRepository categoryRepository) : Controller
    {
        [HttpPost("add"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCategory(Category categoryRequestDTO)
        {
            var result = await categoryRepository.CreateAsync(categoryRequestDTO);
            return Ok(result);
        }

        [HttpDelete("delete/{categoryId}"), Authorize(Roles ="admin")]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            var result = await categoryRepository.DeleteAsync(categoryId);
            return Ok(result);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await categoryRepository.FindAllAsync();
            return Ok(result);
        }

        [HttpGet("getbyid/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(string categoryId)
        {
            var result = await categoryRepository.FindByIdAsync(categoryId);
            return Ok(result);
        }

        [HttpPut("update"),Authorize(Roles ="admin")]
        public async Task<IActionResult> UpdateCategory(Category categoryRequestDTO)
        {
            var result = await categoryRepository.UpdateAsync(categoryRequestDTO);
            return Ok(result);
        }
    }
}
