using LMS.Blazor.Components.Account;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace LMS.Blazor.Services;

internal class ApiCaller(IHttpClientFactory httpClientFactory, ITokenStorage TokenService, AuthenticationStateProvider authentication, IConfiguration config) : IApiCaller
{
    private HttpClient client = null!;
    public async Task<T?> GetAsync<T>(string endPoint, Type returnType, CancellationToken cancellationToken = default)
    {
        await Prepare();
        var baseAddress = config["LmsAPIBaseAddress"];
        ArgumentException.ThrowIfNullOrEmpty(baseAddress, nameof(baseAddress));

        Uri uri = new Uri(new Uri(baseAddress), endPoint);
        return await client.GetFromJsonAsync<T>(uri, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }, cancellationToken);
    }
    private async Task<IActionResult> Prepare()
    {
        var authState = (await authentication.GetAuthenticationStateAsync());
        var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return new UnauthorizedResult();

        var tokens = await TokenService.GetTokensAsync(userId);
        if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken) || string.IsNullOrEmpty(tokens.RefreshToken))
            return new UnauthorizedResult();

        if (string.IsNullOrEmpty(tokens.AccessToken))
        {
            await TokenService.RefreshTokensAsync(userId);
            tokens = await TokenService.GetTokensAsync(userId);
        }

        if (string.IsNullOrEmpty(tokens.AccessToken))
            return new UnauthorizedResult();

        client = httpClientFactory.CreateClient("LmsAPIClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        return new OkResult();
    }
}
