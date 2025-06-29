using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Components.Models;

public class UserDto
{
    public int pn_User_No { get; set; }
    public string v_User_Name { get; set; } = string.Empty;
    public string v_Password { get; set; } = string.Empty;
    public string v_Email { get; set; } = string.Empty;
    public bool b_Is_Active { get; set; }
    public string? v_Role { get; set; }
}

public class UserModel
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Role { get; set; } = string.Empty;
    public string RoleNo { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string token { get; set; } = string.Empty;
    // public UserModel User { get; set; } = new();
    public string message { get; set; } = string.Empty;
}
