namespace BSC.UI.Models;

public sealed class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public sealed record LoginUser(
    int UserId,
    string Name,
    string Email,
    int UserRole,
    bool Active);

public sealed record LoginResponse(
    string Token,
    LoginUser User);
