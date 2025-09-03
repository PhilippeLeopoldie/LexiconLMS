using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services.NoOpServices;

public class ServerNoopApiService : IApiService
{
    public Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default) => Task.FromResult<T?>(default);

    public Task<T?> PostApiAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default) => Task.FromResult<T?>(default);
   
}
