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
            try
            {
                var response = await _userAccount.CreateAccountAsync(registerUserDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDTO)
        {
            try
            {
                var response = await _userAccount.LoginAccountAsync(loginDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateaccount"), Authorize]
        public async Task<IActionResult> Update([FromForm] UpdateUserDTO userDTO)
        {
            try
            {
                var userId = User.GetUserId();
                var response = await _userAccount.UpdateAccountAsync(userId, userDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //return current user details
        [HttpGet("getme"), Authorize]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _userAccount.GetAccountByIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete"), Authorize]
        public async Task<IActionResult> Delete()
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _userAccount.DeleteAccountAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updatepassword"), Authorize]
        public async Task<IActionResult> UpdatePassword([FromForm] UpdatePasswordDTO updatePassword)
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _userAccount.UpdateAccountPasswordAsync(userId, updatePassword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //signin with google
        [HttpPost("googlelogin")]
        public async Task<IActionResult> GoogleLogin([FromForm] GoogleLoginDTO googleLoginDTO)
        {
            try
            {
                var response = await _userAccount.GoogleLoginAsync(googleLoginDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getaccountbyid/{id}"), Authorize]
        public async Task<IActionResult> GetAccountById(string id)
        {
            try
            {
                var result = await _userAccount.GetAccountByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getallusers"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _userAccount.GetAllUsersAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
