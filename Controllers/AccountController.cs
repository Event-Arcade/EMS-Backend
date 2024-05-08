using EMS.BACKEND.API.DTOs.Account;
using EMS.BACKEND.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccount;

        public AccountController(IUserAccountRepository userAccount)
        {
            _userAccount = userAccount;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUserDTO registerUserDTO)
        {
            var response = await _userAccount.CreateAccountAsync(registerUserDTO);
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
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDTO)
        {
            var response = await _userAccount.LoginAccountAsync(loginDTO);
            if (response.Flag)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("updateaccount"), Authorize]
        public async Task<IActionResult> Update([FromForm] UpdateUserDTO userDTO)
        {
            var userId = User.GetUserId();
            var response = await _userAccount.UpdateAccountAsync(userId, userDTO);
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
        [HttpGet(), Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.GetUserId();
            var result = await _userAccount.GetAccountByIdAsync(userId);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete(), Authorize]
        public async Task<IActionResult> Delete()
        {
            var userId = User.GetUserId();
            var result = await _userAccount.DeleteAccountAsync(userId);
            if (result.Flag)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("updatepassword"), Authorize]
        public async Task<IActionResult> UpdatePassword([FromForm] UpdatePasswordDTO updatePassword)
        {
            var userId = User.GetUserId();
            var result = await _userAccount.UpdateAccountPasswordAsync(userId, updatePassword);
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
