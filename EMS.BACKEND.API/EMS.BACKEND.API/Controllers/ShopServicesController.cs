using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Controllers
{

    [Route("api/[controller]"),ApiController,Authorize]
    public class ShopServicesController(IShopServiceRepository shopServiceRepository) : ControllerBase
    {
        [HttpGet("myshop"),Authorize(Roles = "admin , vendor")] 
        public IActionResult Get()
        {
            var response = shopServiceRepository.GetShop();
            return Ok(response);
        }

        [HttpPost("createmyshop"),Authorize]
        public IActionResult CreateShop(ShopRequestDTO shopRequestDTO)
        {
            var responce = shopServiceRepository.CreateShop(shopRequestDTO);
            return Ok(responce);
        }

        [HttpPut("updatemyshop"), Authorize(Roles = "admin , vendor")]
        public IActionResult Put(ServiceRequestDTO platformDTO)
        {
            return Ok("elm");
        }

        [HttpDelete("deletemyshop"), Authorize(Roles = " admin, vendor")]
        public IActionResult Delete()
        {
            return Ok("deleted");
        }
    }
}
