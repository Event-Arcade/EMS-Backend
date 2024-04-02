using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryRepository categoryRepository) : Controller
    {
        [HttpPost("add"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCategory(RequestDTO categoryRequestDTO)
        {
            var result = await categoryRepository.AddCategory(categoryRequestDTO);
            return Ok(result);
        }

        [HttpDelete("delete/{categoryId}"), Authorize(Roles ="admin")]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            var result = await categoryRepository.DeleteCategory(categoryId);
            return Ok(result);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await categoryRepository.GetAllCategories();
            return Ok(result);
        }

        [HttpGet("getbyid/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(string categoryId)
        {
            var result = await categoryRepository.GetCategoryById(categoryId);
            return Ok(result);
        }

        [HttpPut("update"),Authorize(Roles ="admin")]
        public async Task<IActionResult> UpdateCategory(RequestDTO categoryRequestDTO)
        {
            var result = await categoryRepository.UpdateCategory(categoryRequestDTO);
            return Ok(result);
        }
    }
}
