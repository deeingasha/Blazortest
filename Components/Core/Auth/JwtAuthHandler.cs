using BlazorApp1.Components.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorApp1.Components.Core.Auth;

public class JwtAuthHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public JwtAuthHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var sessionStorage = scope.ServiceProvider.GetService<ProtectedSessionStorage>();

            if (sessionStorage != null)
            {
                var tokenResult = await sessionStorage.GetAsync<string>("authToken");
                if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
                {
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResult.Value);
                }
            }
        }
        catch (Exception)
        {
            // Ignore token retrieval errors during HTTP requests
            // No action needed; proceed without setting the Authorization header
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
