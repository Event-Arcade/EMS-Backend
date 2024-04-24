using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Controllers
{

    [Route("api/[controller]"), ApiController]
    public class ShopServicesController(IShopServiceRepository shopServiceRepository) : ControllerBase
    {
        [HttpGet("myshop"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> GetMyShop()
        {
            var response = await shopServiceRepository.GetShopByVendor();
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
        public async Task<IActionResult> CreateShop(Shop shop)
        {
            var response = await shopServiceRepository.CreateAsync(shop);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("updatemyshop"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> UpdateShop([FromQuery] String shopId, [FromForm] Shop shop)
        {
            var response = await shopServiceRepository.UpdateAsync(shopId, shop);
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
        public async Task<IActionResult> DeleteShop(string shopId)
        {
            var response = await shopServiceRepository.DeleteAsync(shopId);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        //get all shops
        [HttpGet("allshops")]
        public async Task<IActionResult> GetAllShops()
        {
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
