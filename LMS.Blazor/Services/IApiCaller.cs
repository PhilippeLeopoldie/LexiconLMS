
namespace LMS.Blazor.Services
{
    internal interface IApiCaller
    {
        Task<T?> GetAsync<T>(string endPoint, Type returnType, CancellationToken cancellationToken = default);
    }
}