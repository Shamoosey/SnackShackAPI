using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SnackShackAPI.Controllers;
using SnackShackAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SnackShackAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private static readonly HttpClient _httpClient = new HttpClient();

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<DiscordTokenResponse> ExchangeCodeForToken(string code)
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

        public async Task<DiscordUserInfo> FetchDiscordUserInfo(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DiscordUserInfo>(content);
        }

        public async Task<bool> IsUserInDiscordServer(string discordUserId, string token)
        {
            var requiredServerId = _config["Discord:RequiredServerId"];
            var guildsUrl = "https://discord.com/api/users/@me/guilds";

            var request = new HttpRequestMessage(HttpMethod.Get, guildsUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();
            var guilds = JsonConvert.DeserializeObject<List<DiscordGuild>>(content);

            return guilds.Any(g => g.Id == requiredServerId);
        }

        public string GenerateJwtToken(DiscordUserInfo userInfo, int expiresInMinutes)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id ?? Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email ?? "Unknown"),
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

    public interface IAuthService
    {
        Task<DiscordTokenResponse> ExchangeCodeForToken(string code);
        Task<DiscordUserInfo> FetchDiscordUserInfo(string accessToken);
        Task<bool> IsUserInDiscordServer(string discordUserId, string token);
        string GenerateJwtToken(DiscordUserInfo userInfo, int expiresInMinutes);
    }
}
