using BlazorApp1.Components.Core.Auth;
using BlazorApp1.Components.Core.Http;
using BlazorApp1.Components.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorApp1.Components.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<UserModel?> GetCurrentUserAsync();
    Task<string?> GetTokenAsync();
    void RegisterStateChangedCallback(Action callback);
}

public class AuthService : IAuthService
{
    private readonly IApiHttpClient _apiClient;
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly ILogger<AuthService> _logger;
    private UserModel? _cachedUser;
    private readonly List<Action> _stateChangedCallbacks = new();
    private const string TokenKey = "authToken";
    private const string UserKey = "currentUser";

    public AuthService(
        IApiHttpClient apiClient,
        ProtectedSessionStorage sessionStorage,
        ILogger<AuthService> logger)
    {
        _apiClient = apiClient;
        _sessionStorage = sessionStorage;
        _logger = logger;
    }

    public void RegisterStateChangedCallback(Action callback)
    {
        _stateChangedCallbacks.Add(callback);
    }

    private void NotifyStateChanged()
    {
        foreach (var callback in _stateChangedCallbacks)
        {
            callback.Invoke();
        }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            _logger.LogDebug("LoginAsync for user {Username}", username);

            var loginRequest = new
            {
                Username = username,
                Password = password,
                RoleNo = "1" // Default role, you might want to make this configurable
            };

            try
            {
                // Try real API first
                var response = await _apiClient.PostAsync<object, LoginResponse>(
                    "api/Login/Authentication", loginRequest);

                if (response?.token != null && response.message == "success")
                {
                    // Handle real API response
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(response.token);

                    var userModel = new UserModel
                    {
                        UserId = jsonToken.Claims.FirstOrDefault(x => x.Type == "entityNumber")?.Value ?? "0",
                        Username = username,
                        Email = $"{username}@hospital.com",
                        Role = jsonToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? "User",
                        RoleNo = "1",
                        IsActive = true
                    };

                    await _sessionStorage.SetAsync(TokenKey, response.token);
                    await _sessionStorage.SetAsync(UserKey, userModel);
                    _cachedUser = userModel;

                    _logger.LogInformation("User {Username} logged in successfully via API", username);
                    NotifyStateChanged();
                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "API server unavailable, falling back to mock authentication");

                // Fallback to mock authentication
                if (username.ToLower() == "admin" && password == "54321")
                {
                    var mockUserModel = new UserModel
                    {
                        UserId = "1",
                        Username = username,
                        Email = $"{username}@hospital.com",
                        Role = "Admin",
                        RoleNo = "1",
                        IsActive = true
                    };

                    var mockToken = "mock-jwt-token-for-development";
                    await _sessionStorage.SetAsync(TokenKey, mockToken);
                    await _sessionStorage.SetAsync(UserKey, mockUserModel);
                    _cachedUser = mockUserModel;

                    _logger.LogInformation("User {Username} logged in successfully via mock fallback", username);
                    NotifyStateChanged();
                    return true;
                }
            }

            _logger.LogWarning("Login failed for user {Username}", username);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Username}", username);
            return false;
        }
    }

    public async Task<UserModel?> GetCurrentUserAsync()
    {
        try
        {
            if (_cachedUser != null)
            {
                return _cachedUser;
            }

            var result = await _sessionStorage.GetAsync<UserModel>(UserKey);
            if (result.Success && result.Value != null)
            {
                _cachedUser = result.Value;
                return _cachedUser;
            }

            return null;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
        {
            // During prerendering, session storage isn't available
            _logger.LogDebug("Cannot access session storage during prerendering");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return null;
        }
    }
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(TokenKey);
            return result.Success ? result.Value : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting token");
            return null;
        }
    }


    public async Task LogoutAsync()
    {
        try
        {
            await _sessionStorage.DeleteAsync(TokenKey);
            await _sessionStorage.DeleteAsync(UserKey);
            _cachedUser = null;
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
    }
}
