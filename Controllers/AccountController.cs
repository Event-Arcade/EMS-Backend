using EMS.BACKEND.API.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserAccountRepository userAccount) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRequestDTO userDTO)
        {
            var response = await userAccount.CreateAccount(userDTO);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var response = await userAccount.LoginAccount(loginDTO);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("update"), Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequestDTO userDTO)
        {
            var response = await userAccount.UpdateAccount(userDTO);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        //return current user details
        [HttpGet("getme"), Authorize]
        public async Task<IActionResult> GetMe()
        {
            var result = await userAccount.GetMe();
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
