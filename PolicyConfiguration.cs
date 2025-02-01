using Microsoft.AspNetCore.Authorization;
using SnackShackAPI.Services;
using System.Security.Claims;

public class DiscordServerMemberPolicyHandler : AuthorizationHandler<DiscordServerMemberRequirement>
{
    private readonly IAuthService _authService;

    public DiscordServerMemberPolicyHandler(IAuthService authService)
    {
        _authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DiscordServerMemberRequirement requirement)
    {
        var discordUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var accessToken = context.User.FindFirst("access_token")?.Value;

        if (string.IsNullOrEmpty(discordUserId) || string.IsNullOrEmpty(accessToken))
        {
            context.Fail();
            return;
        }

        var isUserInServer = await _authService.IsUserInDiscordServer(discordUserId, accessToken);
        if (isUserInServer)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}

public class DiscordServerMemberRequirement : IAuthorizationRequirement
{ }
