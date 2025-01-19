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
            if (string.IsNullOrEmpty(request.Code))
                return BadRequest("Code is required.");

            try
            {
                // Step 1: Exchange code for access token
                var tokenResponse = await ExchangeCodeForToken(request.Code);
                if (tokenResponse == null)
                    return BadRequest("Failed to exchange code for token.");

                // Step 2: Fetch user info from Discord API
                var userInfo = await FetchDiscordUserInfo(tokenResponse.AccessToken);
                if (userInfo == null)
                    return BadRequest("Failed to fetch user info.");

                // Step 3: Generate JWT for your application
                var jwt = GenerateJwtToken(userInfo);

                return Ok(new { token = jwt, user = userInfo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
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
            Console.WriteLine($"Response Content: {await response.Content.ReadAsStringAsync()}");

            if (!response.IsSuccessStatusCode)
                return null;

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

        private string GenerateJwtToken(DiscordUserInfo userInfo)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.Name, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email ?? ""),
                new Claim("Avatar", userInfo.Avatar ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
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
