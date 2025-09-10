namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task PostAsync(string endpoint, object data, CancellationToken cancellationToken = default);
    Task<T?> PostWithResponseAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task PutAsync(string endpoint, object data, CancellationToken cancellationToken = default);
    Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
    Task<(T? Data, string? PaginationHeader)> CallApiWithPaginationAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadFileAsync(string endpoint, CancellationToken cancellationToken = default);
}
