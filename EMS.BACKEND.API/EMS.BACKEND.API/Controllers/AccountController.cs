using EMS.BACKEND.API.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserAccountRepository userAccount) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRequestDTO userDTO)
        {
            var response = await userAccount.CreateAccount(userDTO);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await userAccount.LoginAccount(loginDTO);
            return Ok(response);
        }

        [HttpPut("update"), Authorize]
        public async Task<IActionResult> Update(UserRequestDTO userDTO)
        {
            var response = await userAccount.UpdateAccount(userDTO);
            return Ok(response);
        }

        //return current user details
        [HttpGet("getme"), Authorize]
        public async Task<IActionResult> GetMe()
        {
            var result = await userAccount.GetMe();
            return Ok(result);
        }

        //return current user profile picture
        [HttpGet("getprofilepicture"), Authorize]
        public async Task<IActionResult> GetProfilePicture()
        {
            var result = await userAccount.GetProfilePicture();
            return File(result.bitStream, result.contentType);
        }
    }
}
