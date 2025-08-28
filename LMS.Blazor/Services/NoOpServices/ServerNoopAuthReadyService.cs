using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services.NoOpServices;

public sealed class ServerNoopAuthReadyService : IAuthReadyService
{
    public Task WaitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
