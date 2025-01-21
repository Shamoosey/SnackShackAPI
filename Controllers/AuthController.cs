using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SnackShackAPI.DTOs;
using SnackShackAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using SnackShackAPI.Models;

namespace SnackShackAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public AuthController(IConfiguration config, IAuthService authService, IUserService userService, IAccountService accountService)
        {
            _config = config;
            _authService = authService;
            _userService = userService;
            _accountService = accountService;
        }

        [HttpPost("discord")]
        public async Task<IActionResult> DiscordAuth([FromBody] DiscordAuthRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code))
                    return BadRequest("Request did not include code, unable to complete");

                var tokenResponse = await _authService.ExchangeCodeForToken(request.Code);
                if (tokenResponse == null)
                    throw new Exception("Failed to exchange code for token");

                var userInfo = await _authService.FetchDiscordUserInfo(tokenResponse.AccessToken);
                if (userInfo == null)
                    throw new Exception("Failed to fetch user info");

                var userExists = await _userService.UserExists(userInfo.Email);
                var isUserInServer = await _authService.IsUserInDiscordServer(userInfo.Id, tokenResponse.AccessToken);
                if (!userExists && isUserInServer)
                {
                    var user = new UserDTO
                    {
                        Email = userInfo.Email,
                        Username = userInfo.Username
                    };

                    await _userService.CreateUser(user);
                    var accounts = _config.GetSection("Data:DefaultAccounts").Get<List<DefaultAccountConfig>>();
                    var userDto = await _userService.GetUser(user.Email);
                    foreach (var account in accounts)
                    {
                        await _accountService.CreateAccount(userDto.Id, account.Name, account.StartingAmount, account.CurrencyCode);
                    }
                }

                if (!isUserInServer)
                    throw new Exception("User not in required discord server");

                var accessToken = _authService.GenerateJwtToken(userInfo, 15); // 15 minutes for access token
                var refreshToken = _authService.GenerateJwtToken(userInfo, 1440); // 1 day (1440 minutes) for refresh token

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None, // change before deploy
                    Expires = DateTime.UtcNow.AddDays(1)
                };
                Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

                return Ok(new DiscordAuthResult { Token = accessToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]);
                    tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                    var userInfo = new DiscordUserInfo { Email = email }; // Mock user info
                    var newAccessToken = _authService.GenerateJwtToken(userInfo, 15); // 15 minutes for access token

                    return Ok(new DiscordAuthResult { Token = newAccessToken });
                }
                catch
                {
                    return Unauthorized("Invalid or expired refresh token");
                }
            }

            return Unauthorized("Refresh token not found");
        }
    }
}