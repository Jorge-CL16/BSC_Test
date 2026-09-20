using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace BSC.UI.Services;

public sealed class JwtAuthorizationHandler(IJSRuntime jsRuntime) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await jsRuntime.InvokeAsync<string?>(
            "localStorage.getItem",
            cancellationToken,
            "bsc_access_token");

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
