using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.Category;
using EMS.BACKEND.API.Extensions;
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
        public async Task<IActionResult> AddCategory([FromForm] CategoryRequestDTO categoryRequestDTO)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserId();
            var result = await categoryRepository.CreateAsync(userId, categoryRequestDTO);
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
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserId();
            var result = await categoryRepository.DeleteAsync(userId, categoryId);
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
        public async Task<IActionResult> GetCategoryById(int categoryId)
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
        public async Task<IActionResult> UpdateCategory(int categotyId, [FromForm] CategoryRequestDTO category)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserId();
            var result = await categoryRepository.UpdateAsync(userId, categotyId, category);
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
