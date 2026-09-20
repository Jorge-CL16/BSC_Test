using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BSC.UI.Services;

public sealed class JwtAuthenticationStateProvider(IJSRuntime jsRuntime)
    : AuthenticationStateProvider
{
    private const string TokenKey = "bsc_access_token";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        return CreateAuthenticationState(token);
    }

    public void NotifyUserAuthentication(string token)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(CreateAuthenticationState(token)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(CreateAuthenticationState(null)));
    }

    private static AuthenticationState CreateAuthenticationState(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        try
        {
            var identity = new ClaimsIdentity(
                ParseClaims(token),
                "jwt",
                ClaimTypes.Name,
                ClaimTypes.Role);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (Exception) when (token.Length > 0)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    private static IEnumerable<Claim> ParseClaims(string token)
    {
        var payload = token.Split('.')[1];
        var normalized = payload.Replace('-', '+').Replace('_', '/');
        normalized = normalized.PadRight(normalized.Length + (4 - normalized.Length % 4) % 4, '=');
        using var document = JsonDocument.Parse(
            Encoding.UTF8.GetString(Convert.FromBase64String(normalized)));

        foreach (var property in document.RootElement.EnumerateObject())
        {
            var claimType = property.Name switch
            {
                "sub" => ClaimTypes.NameIdentifier,
                "role" => ClaimTypes.Role,
                "name" => ClaimTypes.Name,
                "email" => ClaimTypes.Email,
                _ => property.Name
            };

            yield return new Claim(claimType, property.Value.ToString());
        }
    }
}
