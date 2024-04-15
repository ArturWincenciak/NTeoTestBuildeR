using JetBrains.Annotations;

namespace TeoTests.Core.Verify;

[UsedImplicitly]
public sealed record Actual(
    string Description,
    Request Request,
    Response Response)
{
    public static Task<Actual> Create(string description,
        HttpRequestMessage httpRequest, HttpResponseMessage httpResponse,
        object? requestPayload, object? responsePayload) =>
        Task.FromResult(new Actual(description,
            Request: Request.Create(httpRequest, requestPayload),
            Response: Response.Create(httpResponse, responsePayload)));
}