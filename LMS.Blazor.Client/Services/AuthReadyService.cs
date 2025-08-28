using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LMS.Blazor.Client.Services;

public sealed class AuthReadyService : IAuthReadyService
{
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly TaskCompletionSource<bool> readyTaskCompletionSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    // Set max timeout here. Now 5 seconds before error
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    public AuthReadyService(AuthenticationStateProvider authenticationStateProvider)
    {
        this.authenticationStateProvider = authenticationStateProvider;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        if (!OperatingSystem.IsBrowser())
            return;

        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        if (IsAuthenticated(state.User))
            readyTaskCompletionSource.TrySetResult(true);

        authenticationStateProvider.AuthenticationStateChanged += async t =>
        {
            var state = await t;
            if (IsAuthenticated(state.User))
                readyTaskCompletionSource.TrySetResult(true);
        };
    }

    private static bool IsAuthenticated(ClaimsPrincipal principal) =>
      principal.Identity?.IsAuthenticated == true;

    public async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        var completed = await Task.WhenAny(readyTaskCompletionSource.Task, Task.Delay(DefaultTimeout, cancellationToken));
        if (completed != readyTaskCompletionSource.Task)
            throw new TimeoutException("Authentication state was not ready in time. ({DefaultTimeout} seconds).");

        await readyTaskCompletionSource.Task;
    }
}