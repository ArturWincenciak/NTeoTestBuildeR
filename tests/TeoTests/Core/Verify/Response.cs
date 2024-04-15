using System.Net;
using JetBrains.Annotations;

namespace TeoTests.Core.Verify;

[UsedImplicitly]
public sealed record Response(
    HttpStatusCode StatusCode,
    IReadOnlyDictionary<string, IEnumerable<string>> Headers,
    object? Payload = null)
{
    public static Response Create(HttpResponseMessage httpResponse, object? result = null) =>
        new(httpResponse.StatusCode,
            Headers: httpResponse.Headers.Map(),
            result);
}