using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnackShackAPI.DTOs;
using SnackShackAPI.Models;
using SnackShackAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SnackShackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("GetByUser")]
        public async Task<IActionResult> GetAccounts()
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var accounts = await _accountService.GetAccountsByDiscordUser(userId);

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the users accounts.", error = ex.Message });
            }
        }

        [HttpGet("GetAccountHistory/{accountId}")]
        public async Task<IActionResult> GetAccountHistory(Guid accountId)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var accounts = await _accountService.GetAccountHistory(userId, accountId);

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user account history.", error = ex.Message });
            }
        }

        [HttpPut("TransferFunds")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsRequest data)
        {
            try
            {
                var result = await this._accountService.TransferFunds(data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the users accounts.", error = ex.Message });
            }
        }

        [HttpPost("UpdateAccountBalance")]
        public async Task<IActionResult> UpdateAccountBalance([FromBody]UpdateAccountBalanceRequest request) 
        {
            try
            {
                var accounts = await _accountService.UpdateAccountBalance(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the users accounts.", error = ex.Message });
            }
        }
    }
}
