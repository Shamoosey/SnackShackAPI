using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SnackShackAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SnackShackAPI
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        private readonly IAuthService _authService;

        public CustomJwtBearerEvents(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken == null)
            {
                context.Fail("Invalid token");
                return;
            }

            var userInfo = await _authService.FetchDiscordUserInfo(accessToken.RawData);
            if (userInfo == null)
            {
                context.Fail("Token validation failed: User not found");
            }
            else
            {
                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id)
            }, JwtBearerDefaults.AuthenticationScheme));
            }
        }
    }

}
