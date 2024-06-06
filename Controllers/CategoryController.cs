using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.Category;
using EMS.BACKEND.API.Enums;
using EMS.BACKEND.API.Extensions;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotificationRepository _notificationRepository;

        public CategoryController(ICategoryRepository categoryRepository, INotificationRepository notificationRepository)
        {
            _categoryRepository = categoryRepository;
            _notificationRepository = notificationRepository;
        }

        [HttpPost("create"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCategory([FromForm] CategoryRequestDTO categoryRequestDTO)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.GetUserId();
                var result = await _categoryRepository.CreateAsync(userId, categoryRequestDTO);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

            try
            {
                var userId = User.GetUserId();
                var result = await _categoryRepository.DeleteAsync(userId, categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var result = await _categoryRepository.FindAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("get/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            try
            {
                var result = await _categoryRepository.FindByIdAsync(categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

            try
            {
                var userId = User.GetUserId();
                var result = await _categoryRepository.UpdateAsync(userId, categotyId, category);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
