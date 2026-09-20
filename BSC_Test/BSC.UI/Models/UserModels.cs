namespace BSC.UI.Models;

public sealed record UserDto(
    int UserId,
    string Name,
    string Email,
    int UserRole,
    bool Active);

public sealed class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int UserRole { get; set; } = 3;
}
