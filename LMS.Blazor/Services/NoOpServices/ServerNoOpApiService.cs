using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services.NoOpServices;

public class ServerNoopApiService : IApiService
{
    public Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default) => Task.FromResult<T?>(default);
    public Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task PostAsync(string endpoint, object data, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task PutAsync(string endpoint, object data, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
