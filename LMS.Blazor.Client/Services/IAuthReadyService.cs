
namespace LMS.Blazor.Client.Services;

public interface IAuthReadyService
{
    Task WaitAsync(CancellationToken cancellationToken = default);
}