using System.Diagnostics;
using TeoTests.Core.Verify;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Request = WireMock.RequestBuilders.Request;
using Response = WireMock.ResponseBuilders.Response;

namespace TeoTests.Core;

public abstract class TestBuilder<TBuilder>
    where TBuilder : TestBuilder<TBuilder>
{
    private readonly Activity _activity = new Activity(Guid.NewGuid().ToString()).Start();
    private readonly HttpClient _httpClient = App.HttpClient;
    private readonly List<Func<Task<object?>>> _steps = [];

    protected void With(Func<Task<object?>> step) =>
        _steps.Add(step);

    public async Task<List<object?>> Build()
    {
        try
        {
            var result = new List<object?>();

            foreach (var step in _steps)
                result.Add(await step());

            return result;
        }
        finally
        {
            _activity.Dispose();
        }
    }

    protected Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        request.Headers.Add(name: "traceparent", _activity.Id);
        return _httpClient.SendAsync(request);
    }

    internal TBuilder WithWiremock(Action<(IRequestBuilder request, IResponseBuilder response)> configure)
    {
        BuildWiremock(configure);
        return (TBuilder) this;
    }

    private void BuildWiremock(Action<(IRequestBuilder request, IResponseBuilder response)> configure)
    {
        var requestBuilder = Request.Create().WithHeader(name: "traceparent", pattern: $"*{_activity.TraceId}*");
        var responseBuilder = Response.Create();

        configure((requestBuilder, responseBuilder));

        App.Wiremock
            .Given(requestBuilder)
            .RespondWith(responseBuilder);
    }
}