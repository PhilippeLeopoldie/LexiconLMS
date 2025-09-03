using Microsoft.AspNetCore.Mvc;

namespace LMS.Blazor.Controller;

[Route("proxy")]
[ApiController]
public class ProxyController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<IActionResult> Proxy(string endpoint, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(endpoint);

        var client = _httpClientFactory.CreateClient("LmsAPIClient");

        var rawQuery = Request.QueryString.Value;
        string endpointPath = endpoint;
        string extraQuery = string.Empty;

        if (!string.IsNullOrEmpty(rawQuery))
        {
            int firstQ = rawQuery.IndexOf('?');
            int secondQ = rawQuery.IndexOf('?', firstQ + 1);

            if (secondQ >= 0 && secondQ + 1 < rawQuery.Length)
                extraQuery = rawQuery.Substring(secondQ + 1);
        }

        var targetUriBuilder = new UriBuilder($"{client.BaseAddress}{endpointPath}");
        if (!string.IsNullOrEmpty(extraQuery))
            targetUriBuilder.Query = extraQuery;

        var method = new HttpMethod(Request.Method);
        var requestMessage = new HttpRequestMessage(method, targetUriBuilder.Uri);

        if (method != HttpMethod.Get && Request.ContentLength > 0)
        {
            requestMessage.Content = new StreamContent(Request.Body);

            if (!string.IsNullOrWhiteSpace(Request.ContentType))
            {
                requestMessage.Content.Headers.TryAddWithoutValidation("Content-Type", Request.ContentType);
            }
        }

        foreach (var header in Request.Headers)
        {
            if (!header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
            {
                if (requestMessage.Content != null && header.Key.StartsWith("Content-", StringComparison.OrdinalIgnoreCase))
                {
                    requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
                else
                {
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }
        }

        var response = await client.SendAsync(requestMessage, cancellationToken);

        return !response.IsSuccessStatusCode
            ? StatusCode((int)response.StatusCode)
            : StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
    }
}
