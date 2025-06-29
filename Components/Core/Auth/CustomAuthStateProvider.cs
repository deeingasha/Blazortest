using BlazorApp1.Components.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorApp1.Components.Core.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly ILogger<CustomAuthStateProvider> _logger;

    public CustomAuthStateProvider(IAuthService authService, ILogger<CustomAuthStateProvider> logger)
    {
        _authService = authService;
        _logger = logger;

        (_authService as AuthService)?.RegisterStateChangedCallback(async () =>
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        });
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _authService.GetCurrentUserAsync();

        if (user == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Name, user.Username),
            // new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleNo)
            // new Claim(ClaimTypes.RoleNo, user.RoleNo)
        };

        var identity = new ClaimsIdentity(claims, "ServerAuth");
        _logger.LogInformation("User authenticated: {Username}", user.Username);
        foreach (var claim in claims)
        {
            _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
