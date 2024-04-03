
using EMS.BACKEND.API.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Controllers
{

    [Route("api/[controller]"), ApiController]
    public class ShopServicesController(IShopServiceRepository shopServiceRepository) : ControllerBase
    {
        [HttpGet("myshop"), Authorize(Roles ="vendor")]
        public async Task<IActionResult> GetMyShop()
        {
            // var response = await shopServiceRepository.GetMyShop();
            // if (response == null)
            // {
            //     return BadRequest(response);
            // }
            return Ok();
        }

        [HttpPost("createmyshop"), Authorize(Roles = "client")]
        public async Task<IActionResult> CreateShop(ShopRequestDTO shopRequestDTO)
        {
           // var response = await shopServiceRepository.CreateShop(shopRequestDTO);
            return Ok();
        }

        [HttpPut("updatemyshop"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> UpdateShop(ShopRequestDTO shopRequestDTO)
        {
            // var response = await shopServiceRepository.UpdateShop(shopRequestDTO);
            // if (response == null)
            // {
            //     return BadRequest(response);
            // }
            return Ok();
        }

        [HttpDelete("deletemyshop"), Authorize(Roles = "vendor")]
        public async Task<IActionResult> DeleteShop()
        {
            // var response = await shopServiceRepository.DeleteShop();
            // if (response == null)
            // {
            //     return BadRequest(response);
            // }
            return Ok();
        }

        //get all shops
        [HttpGet("allshops")]
        public async Task<IActionResult> GetAllShops()
        {
            // var response = await shopServiceRepository.GetAllShops();
            // if (response == null)
            // {
            //     return BadRequest(response);
            // }
            return Ok();
        }

    }
}
