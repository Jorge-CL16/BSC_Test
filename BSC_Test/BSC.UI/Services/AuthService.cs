using System.Net.Http.Json;
using BSC.UI.Models;
using Microsoft.JSInterop;

namespace BSC.UI.Services;

public sealed class AuthService(
    HttpClient httpClient,
    IJSRuntime jsRuntime,
    JwtAuthenticationStateProvider authenticationStateProvider)
{
    private const string TokenKey = "bsc_access_token";

    public async Task<(bool Success, string? Error)> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return (false, "Correo o contraseña incorrectos.");

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);

        if (result is null || string.IsNullOrWhiteSpace(result.Token))
            return (false, "La API no devolvió un token válido.");

        await jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);
        await jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            "bsc_user",
            System.Text.Json.JsonSerializer.Serialize(result.User));
        authenticationStateProvider.NotifyUserAuthentication(result.Token);

        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "bsc_user");
        authenticationStateProvider.NotifyUserLogout();
    }
}
