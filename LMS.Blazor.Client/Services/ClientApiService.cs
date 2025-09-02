using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class ClientApiService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager, IAuthReadyService authReady) : IApiService
{
    private readonly HttpClient httpClient = httpClientFactory.CreateClient("BffClient");

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        await authReady.WaitAsync();
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"proxy?endpoint={endpoint}");
        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden
           || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo("AccessDenied");
        }

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions, CancellationToken.None) ?? default;
    }

    public async Task<T?> PostApiAsync<T>(string endpoint,object data, CancellationToken cancellationToken = default)
    {
        await authReady.WaitAsync();
        var json = JsonSerializer.Serialize(data, _jsonSerializerOptions);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"proxy?endpoint={endpoint}")
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden
           || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo("AccessDenied");
        }

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<T>(
            await response.Content.ReadAsStreamAsync(),
            _jsonSerializerOptions,
            CancellationToken.None
            );
    }
}
