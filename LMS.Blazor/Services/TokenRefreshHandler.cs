using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace LMS.Blazor.Services;

public class TokenRefreshHandler(ITokenStorage tokenStorage, IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return await base.SendAsync(request, cancellationToken);

        var accessToken = await tokenStorage.GetAccessTokenAsync(userId);
        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            try
            {
                using var refreshCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await tokenStorage.RefreshTokensAsync(userId);

                var newAccessToken = await tokenStorage.GetAccessTokenAsync(userId);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);

                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }
        return response;
    }
}
