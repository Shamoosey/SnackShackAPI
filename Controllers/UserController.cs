using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnackShackAPI.DTOs;
using SnackShackAPI.Models;
using SnackShackAPI.Services;
using System.Security.Claims;

namespace SnackShackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        public UserController(IUserService userService, IAccountService accountService, IConfiguration config)
        {
            _userService = userService;
            _accountService = accountService;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var users = await _userService.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, new { message = "An error occurred while retrieving users.", error = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new { message = "Invalid or missing claim in token." });
                }

                var user = await _userService.GetUserByDiscord(userId);

                //If no user is found, create new user with discordId and email from token 
                if (user == null)
                {
                    var result = await this._userService.CreateUser(userId, email);
                    if (!result)
                    {
                        throw new Exception("An error occurred while creating the user.");
                    }

                    //create default accounts from config for user
                    var accounts = _config.GetSection("Data:DefaultAccounts").Get<List<DefaultAccountConfig>>() ?? [];
                    user = await _userService.GetUserByDiscord(userId);
                    foreach (var account in accounts)
                    {
                        await _accountService.CreateAccount(user.Id, account.Name, account.StartingAmount, account.CurrencyCode);
                    }
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the current user.", error = ex.Message });
            }
        }

    }
}
