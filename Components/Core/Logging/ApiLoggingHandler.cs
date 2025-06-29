using System;

using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BlazorApp1.Components.Core.Logging;

public class ApiLoggingHandler : DelegatingHandler
{
    private readonly ILogger<ApiLoggingHandler> _logger;

    public ApiLoggingHandler(ILogger<ApiLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content != null)
        {
            var requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("{Method} Request to {Uri} with body: {Body}",
                request.Method,
                request.RequestUri,
                requestBody);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.Content != null)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            // If GET and response is JSON array, log only first/last items (avoid duplicate if only one)
            if (request.Method == HttpMethod.Get && response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        var array = doc.RootElement;
                        int count = array.GetArrayLength();
                        int take = Math.Min(1, count);

                        var firstItems = array.EnumerateArray().Take(take).ToArray();
                        string summary;
                        if (count > 1)
                        {
                            var lastItems = array.EnumerateArray().Skip(Math.Max(0, count - take)).Take(take).ToArray();
                            summary = $@"{{
  ""Count"": {count},
  ""First"": {JsonSerializer.Serialize(firstItems)},
  ""Last"": {JsonSerializer.Serialize(lastItems)}
}}";
                        }
                        else
                        {
                            summary =
$@"{{
  ""Count"": {count},
  ""First"": {JsonSerializer.Serialize(firstItems)}
}}";
                        }

                        _logger.LogDebug("{Method} Response from {Uri}: {StatusCode} with summary: {Summary}",
                            request.Method,
                            request.RequestUri,
                            response.StatusCode,
                            summary);
                    }
                    else
                    {
                        _logger.LogDebug("{Method} Response from {Uri}: {StatusCode} with body: {Body}",
                            request.Method,
                            request.RequestUri,
                            response.StatusCode,
                            responseBody);
                    }
                }
                catch (JsonException)
                {
                    _logger.LogDebug("{Method} Response from {Uri}: {StatusCode} with body: {Body}",
                    request.Method,
                    request.RequestUri,
                    response.StatusCode,
                    responseBody);
                }
            }
            else
            {
                _logger.LogDebug("{Method} Response from {Uri}: {StatusCode} with body: {Body}",
                    request.Method,
                    request.RequestUri,
                    response.StatusCode,
                    responseBody);
            }
        }

        return response;
    }
}
