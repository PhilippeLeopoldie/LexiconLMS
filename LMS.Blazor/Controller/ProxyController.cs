using Microsoft.AspNetCore.Mvc;

namespace LMS.Blazor.Controller;

[Route("proxy")]
[ApiController]
public class ProxyController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(string endpoint, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(endpoint);

        var client = _httpClientFactory.CreateClient("LmsAPIClient");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);

        if (Request.HasFormContentType)
        {
            var formContent = new MultipartFormDataContent();

            var form = await Request.ReadFormAsync(cancellationToken);

            foreach (var file in form.Files)
            {
                if (file.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream, cancellationToken);
                    var fileBytes = memoryStream.ToArray();

                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                    formContent.Add(fileContent, "file", file.FileName);
                }
            }

            foreach (var field in form)
            {
                if (field.Key != "endpoint")
                {
                    foreach (var value in field.Value)
                    {
                        formContent.Add(new StringContent(value), field.Key);
                    }
                }
            }

            requestMessage.Content = formContent;
        }


        var response = await client.SendAsync(requestMessage, cancellationToken);

        foreach (var header in response.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }
        foreach (var header in response.Content.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }
        Response.Headers.Remove("transfer-encoding");

        var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        Response.StatusCode = (int)response.StatusCode;
        return File(contentStream, contentType);
    }

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

        foreach (var header in response.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }
        foreach (var header in response.Content.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }
        Response.Headers.Remove("transfer-encoding");

        var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        Response.StatusCode = (int)response.StatusCode;
        return File(contentStream, contentType);
    }
}
