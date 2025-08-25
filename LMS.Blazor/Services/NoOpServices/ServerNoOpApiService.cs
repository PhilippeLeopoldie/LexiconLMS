using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services.NoOpServices;

public class ServerNoOpApiService : IApiService
{
    public Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default) => Task.FromResult<T?>(default);
}
