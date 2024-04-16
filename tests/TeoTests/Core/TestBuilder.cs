using TeoTests.Core.Verify;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Request = WireMock.RequestBuilders.Request;
using Response = WireMock.ResponseBuilders.Response;

namespace TeoTests.Core;

public abstract class TestBuilder<TBuilder>
    where TBuilder : TestBuilder<TBuilder>
{
    private readonly HttpClient _httpClient = App.HttpClient;
    private readonly List<Func<Task<object?>>> _steps = [];

    protected void With(Func<HttpClient, Task<object?>> step) =>
        _steps.Add(() => step(_httpClient));

    public async Task<List<object?>> Build()
    {
        var result = new List<object?>();

        foreach (var step in _steps)
            result.Add(await step());

        return result;
    }

    internal TBuilder WithWiremock(Action<(IRequestBuilder request, IResponseBuilder response)> configure)
    {
        BuildWiremock(configure);
        return (TBuilder) this;
    }

    private void BuildWiremock(Action<(IRequestBuilder request, IResponseBuilder response)> configure)
    {
        var requestBuilder = Request.Create();
        var responseBuilder = Response.Create();

        configure((requestBuilder, responseBuilder));

        App.Wiremock
            .Given(requestBuilder)
            .RespondWith(responseBuilder);
    }
}