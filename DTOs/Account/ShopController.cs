using EMS.BACKEND.API.DTOs.Shop;
using EMS.BACKEND.API.Extensions;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Controllers
{

    [Route("api/[controller]"), ApiController]
    public class ShopController(IShopRepository shopServiceRepository) : ControllerBase
    {
        [HttpGet("myshop"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> GetMyShop()
        {
            // model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.GetUserId();
            var response = await shopServiceRepository.GetShopByVendor(userId);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("createmyshop"), Authorize(Roles = "client")]
        public async Task<IActionResult> CreateShop([FromForm] ShopCreateDTO shop)
        {
            // model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.GetUserId();
            var response = await shopServiceRepository.CreateAsync(userId, shop);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("updatemyshop/{shopId}"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> UpdateShop(int shopId, [FromForm] ShopCreateDTO shop)
        {
            // model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.GetUserId();
            var response = await shopServiceRepository.UpdateAsync(userId, shopId, shop);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpDelete("deletemyshop/{shopId}"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> DeleteShop(int shopId)
        {
            // model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.GetUserId();
            var response = await shopServiceRepository.DeleteAsync(userId, shopId);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        [HttpGet("allshops")]
        public async Task<IActionResult> GetAllShops()
        {
            // model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await shopServiceRepository.FindAllAsync();
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
