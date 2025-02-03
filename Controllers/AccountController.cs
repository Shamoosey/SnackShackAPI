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
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("GetByUserGetByUser/{userId}")]
        public async Task<IActionResult> GetAccount(Guid userId)
        {
            try
            {
                var accounts = await _accountService.GetAccountsByUser(userId);

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the users accounts.", error = ex.Message });
            }
        }

        [HttpPut("UpdateAccountInfo/{accountId}")]
        public async Task<IActionResult> UpdateAccountInformation(Guid accountId, [FromBody] UpdateAccountInfomationRequest data)
        {
            try
            {
                var result = await _accountService.UpdateAccountInformation(accountId, data);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the users accounts.", error = ex.Message });
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
