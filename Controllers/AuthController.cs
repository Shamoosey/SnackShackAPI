using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SnackShackAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private static readonly HttpClient _httpClient = new HttpClient();

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("discord")]
        public async Task<IActionResult> DiscordAuth([FromBody] DiscordAuthRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code))
                {
                    throw new Exception("Request did not include code, unable to complete");
                }
         
                var tokenResponse = await ExchangeCodeForToken(request.Code);
                if (tokenResponse == null)
                {
                    throw new Exception("Failed to exchange code for token");
                }

                var userInfo = await FetchDiscordUserInfo(tokenResponse.AccessToken);
                if (userInfo == null)
                {
                    throw new Exception("Failed to fetch user info");
                }

                var accessToken = GenerateJwtToken(userInfo, 15); // 15 minutes for access token
                var refreshToken = GenerateJwtToken(userInfo, 1440); // 1 day (1440 minutes) for refresh token

                //refresh token as an HTTP-only secure cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None, //change before deploy
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
            // Step 1: Retrieve refresh token from the cookie
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                try
                {
                    // Step 2: Validate refresh token
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

                    // Step 3: Generate a new access token
                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var username = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value;
                    var userInfo = new DiscordUserInfo { Username = username }; // Mock user info
                    var newAccessToken = GenerateJwtToken(userInfo, 15); // 15 minutes for access token

                    return Ok(newAccessToken);
                }
                catch
                {
                    return Unauthorized("Invalid or expired refresh token");
                }
            }

            return Unauthorized("Refresh token not found");
        }

        private async Task<DiscordTokenResponse> ExchangeCodeForToken(string code)
        {
            var discordTokenUrl = "https://discord.com/api/oauth2/token";
            var clientId = _config["Discord:ClientId"];
            var clientSecret = _config["Discord:ClientSecret"];
            var redirectUri = _config["Discord:RedirectUri"];

            var payload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri)
            });

            var response = await _httpClient.PostAsync(discordTokenUrl, payload);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DiscordTokenResponse>(content);
        }

        private async Task<DiscordUserInfo> FetchDiscordUserInfo(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DiscordUserInfo>(content);
        }

        private string GenerateJwtToken(DiscordUserInfo userInfo, int expiresInMinutes)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id ?? Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userInfo.Username ?? "Unknown"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class DiscordAuthResult
    {
        public string Token { get; set; }
    }

    public class DiscordAuthRequest
    {
        public string Code { get; set; }
    }

    public class DiscordTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

    public class DiscordUserInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
