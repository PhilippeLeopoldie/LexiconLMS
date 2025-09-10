using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class ClientApiService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager, IAuthReadyService authReady) : IApiService
{
    private readonly HttpClient httpClient = httpClientFactory.CreateClient("BffClient");

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, object? data = null, CancellationToken cancellationToken = default)
    {
        await authReady.WaitAsync();
        var requestMessage = new HttpRequestMessage(method, $"proxy?endpoint={endpoint}");
        if (data is not null)
        {
            requestMessage.Content = JsonContent.Create(data);
        }

        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden
           || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo("/Account/AccessDenied");
            var unauthorizedMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.IsNullOrWhiteSpace(unauthorizedMessage) ? "Access denied" : unauthorizedMessage, null, response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }

        return response;
    }

    public async Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendAsync(HttpMethod.Get, endpoint, null, cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions, CancellationToken.None) ?? default;
    }

    public async Task<(T? Data, string? PaginationHeader)> CallApiWithPaginationAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendAsync(HttpMethod.Get, endpoint, null, cancellationToken);
        var paginationHeader = response.Headers.TryGetValues("X-Pagination", out var values)
            ? values.FirstOrDefault()
            : null;

        var data = await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions, CancellationToken.None) ?? default;
        return (data, paginationHeader);
    }

    public async Task PostAsync(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        await SendAsync(HttpMethod.Post, endpoint, data, cancellationToken);
    }

    public async Task<T?> PostWithResponseAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        var response = await SendAsync(HttpMethod.Post, endpoint, data, cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions, CancellationToken.None) ?? default;
    }

    public async Task PutAsync(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        await SendAsync(HttpMethod.Put, endpoint, data, cancellationToken);
    }

    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        await SendAsync(HttpMethod.Delete, endpoint, null, cancellationToken);
    }

    public async Task<byte[]> DownloadFileAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendAsync(HttpMethod.Get, endpoint, null, cancellationToken);
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<T?> UploadFileAsync<T>(string endpoint, Stream fileStream, string fileName, string contentType, Dictionary<string, string>? formData = null, CancellationToken cancellationToken = default)
    {
        await authReady.WaitAsync();

        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        var fileBytes = memoryStream.ToArray();

        using var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);

        if (formData != null)
        {
            foreach (var kvp in formData)
            {
                content.Add(new StringContent(kvp.Value), kvp.Key);
            }
        }

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"proxy/upload?endpoint={endpoint}")
        {
            Content = content
        };

        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden
           || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo("AccessDenied");
            var unauthorizedMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.IsNullOrWhiteSpace(unauthorizedMessage) ? "Access denied" : unauthorizedMessage, null, response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase : errorBody;
            throw new HttpRequestException(message, null, response.StatusCode);
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)await response.Content.ReadAsStringAsync();
        }

        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions, cancellationToken);
    }
}
