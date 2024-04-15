using System.Net.Http.Headers;
using System.Net.Http.Json;
using Argon;
using JetBrains.Annotations;
using TeoTests.Core.Verify;

namespace TeoTests.Modules.TodosModule.Builder;

internal static class Extensions
{
    [PublicAPI]
    public async static Task<Actual> DeserializeWith<T>(this HttpResponseMessage httpResponse,
        Func<T?, Task<Actual>> success, string description,
        HttpRequestMessage httpRequest, object? requestPayload = null) =>
        httpResponse.IsSuccessStatusCode
            ? await success(await httpResponse.Deserialize<T>())
            : await Actual.Create(description, httpRequest, httpResponse, requestPayload,
                responsePayload: await httpResponse.Deserialize());

    [PublicAPI]
    public async static Task<Actual> DeserializeWith<T>(this HttpResponseMessage httpResponse,
        Action<T?> success, string description, HttpRequestMessage httpRequest, object? requestPayload = null)
    {
        if (httpResponse.IsSuccessStatusCode)
        {
            var responsePayload = await httpResponse.Deserialize<T>();
            success(responsePayload);
            return await Actual.Create(description, httpRequest, httpResponse, requestPayload, responsePayload);
        }

        var problemDetails = await httpResponse.Deserialize();
        return await Actual.Create(description, httpRequest, httpResponse, requestPayload, problemDetails);
    }

    [PublicAPI]
    public async static Task<Actual> DeserializeWith(this HttpResponseMessage httpResponse,
        Action success, string description, HttpRequestMessage httpRequest, object? requestPayload = null)
    {
        if (httpResponse.IsSuccessStatusCode)
        {
            success();
            return await Actual.Create(description, httpRequest, httpResponse, requestPayload, responsePayload: null);
        }

        var problemDetails = await httpResponse.Deserialize();
        return await Actual.Create(description, httpRequest, httpResponse, requestPayload, problemDetails);
    }

    [PublicAPI]
    public async static Task<Actual> Deserialize(this HttpResponseMessage httpResponse,
        string description, HttpRequestMessage httpRequest, object? requestPayload = null)
    {
        var responsePayload = await httpResponse.Deserialize();
        return await Actual.Create(description, httpRequest, httpResponse, requestPayload, responsePayload);
    }

    [PublicAPI]
    public static IReadOnlyDictionary<string, IEnumerable<string>> Map(
        this HttpHeaders requestHeaders) =>
        requestHeaders.ToDictionary(
            keySelector: header => header.Key,
            elementSelector: header => header.Value);

    private async static Task<T?> Deserialize<T>(this HttpResponseMessage httResponse) =>
        await httResponse.Content.ReadFromJsonAsync<T>();

    private async static Task<object?> Deserialize(this HttpResponseMessage httpResponse)
    {
        if ((await httpResponse.Content.ReadAsStreamAsync()).Length is 0)
            return null;

        var jsonString = await httpResponse.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(jsonString);
    }
}