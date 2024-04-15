using JetBrains.Annotations;

namespace TeoTests.Core.Verify;

[UsedImplicitly]
public sealed record Request(
    HttpMethod Method,
    string Path,
    IReadOnlyDictionary<string, IEnumerable<string>> Headers,
    object? Payload = null)
{
    public static Request Create(HttpRequestMessage httpRequest, object? requestPayload = null) =>
        new(httpRequest.Method,
            httpRequest.RequestUri!.AbsoluteUri,
            Headers: httpRequest.Headers.Map(),
            requestPayload);
}