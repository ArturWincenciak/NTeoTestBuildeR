using System.Diagnostics;
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
    private readonly Dictionary<string, (IRequestBuilder Builder, int ExpectedCallCount)> _wiremockConfigs = new();

    protected void With(Func<Task<object?>> step) =>
        _steps.Add(step);

    public async Task<List<object?>> Build()
    {
        try
        {
            var result = new List<object?>();

            foreach (var step in _steps)
                result.Add(await step());

            AssertWiremockCallsCount();

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

    internal TBuilder WithWiremock(Action<(IRequestBuilder request, IResponseBuilder response)> configure,
        int expectedCallCount = 1)
    {
        BuildWiremock(configure, expectedCallCount);
        return (TBuilder) this;
    }

    private void BuildWiremock(Action<(IRequestBuilder request, IResponseBuilder response)> configure,
        int expectedCallCount)
    {
        var requestBuilder = Request.Create().WithHeader(name: "traceparent", pattern: $"*{_activity.TraceId}*");
        var responseBuilder = Response.Create();

        configure((requestBuilder, responseBuilder));

        var id = Guid.NewGuid().ToString();
        App.Wiremock
            .Given(requestBuilder)
            .WithTitle(id)
            .RespondWith(responseBuilder);

        _wiremockConfigs.Add(id, value: (requestBuilder, expectedCallCount));
    }

    private void AssertWiremockCallsCount()
    {
        foreach (var config in _wiremockConfigs)
        {
            var logs = App.Wiremock.LogEntries
                .Where(log => log.MappingTitle == config.Key)
                .ToArray();

            if (logs.Length == config.Value.ExpectedCallCount == false)
                throw new InvalidOperationException(
                    $"The Wiremock configuration {config.Key} is not used as expected. " +
                    $"Expected: {config.Value.ExpectedCallCount}, Actual: {logs.Length}. " +
                    $"Ensure all mocks are used at least once during the test..");
        }
    }
}