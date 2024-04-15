using System.Net.Http.Headers;
using JetBrains.Annotations;

namespace TeoTests.Core.Verify;

public static class Extensions
{
    [PublicAPI]
    public static IReadOnlyDictionary<string, IEnumerable<string>> Map(
        this HttpHeaders requestHeaders) =>
        requestHeaders.ToDictionary(
            keySelector: header => header.Key,
            elementSelector: header => header.Value);
}