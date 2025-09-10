using LMS.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace LMS.Blazor.Services;

public class ServerApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private NavigationManager navigationManager;

    public ServerApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("LmsAPIClient");
    }

    public async Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {

        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden
          || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var unauthorizedMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.IsNullOrWhiteSpace(unauthorizedMessage) ? "Access denied" : unauthorizedMessage, null, response.StatusCode);
        }
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }

        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken), _jsonSerializerOptions, cancellationToken);
    }

    public async Task<(T? Data, string? PaginationHeader)> CallApiWithPaginationAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
        var paginationHeader = response.Headers.TryGetValues("X-Pagination", out var values)
            ? values.FirstOrDefault()
            : null;

        var data = await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken), _jsonSerializerOptions, cancellationToken) ?? default;
        return (data, paginationHeader);
    }

    public async Task PostAsync(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonSerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<T?> PostWithResponseAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonSerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }

        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken), _jsonSerializerOptions, cancellationToken);
    }

    public async Task PutAsync(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonSerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<byte[]> DownloadFileAsync(string endpoint, CancellationToken cancellationToken = default)
    {

        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden
          || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var unauthorizedMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.IsNullOrWhiteSpace(unauthorizedMessage) ? "Access denied" : unauthorizedMessage, null, response.StatusCode);
        }
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }


        return await response.Content.ReadAsByteArrayAsync();
    }
}

