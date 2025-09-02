namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> CallApiAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<T?> PostApiAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
}
