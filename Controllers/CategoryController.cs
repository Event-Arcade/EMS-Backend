using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryRepository categoryRepository) : Controller
    {
        [HttpPost("create"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCategory([FromForm] Category category)
        {
            var result = await categoryRepository.CreateAsync(category);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("delete/{categoryId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            var result = await categoryRepository.DeleteAsync(categoryId);
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
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await categoryRepository.FindAllAsync();
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("get/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(string categoryId)
        {
            var result = await categoryRepository.FindByIdAsync(categoryId);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("update/{categotyId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategory( string categotyId,[FromForm] Category category)
        {
            Console.WriteLine(categotyId);
            var result = await categoryRepository.UpdateAsync(categotyId,category);
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
