using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services.NoOpServices;

public class ServerNoopApiService : IApiService
{
    public Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default) => Task.FromResult<T?>(default);
    public Task<(T? Data, string? PaginationHeader)> CallApiWithPaginationAsync<T>(string endpoint, CancellationToken cancellationToken = default) => Task.FromResult<(T? Data, string? PaginationHeader)>(default);
    public Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<byte[]?> DownloadFileAsync(string endpoint, CancellationToken cancellationToken = default) => Task.FromResult<byte[]?>(default);
    public Task PostAsync(string endpoint, object data, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<T?> PostWithResponseAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default) => Task.FromResult<T?>(default);
    public Task PutAsync(string endpoint, object data, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
