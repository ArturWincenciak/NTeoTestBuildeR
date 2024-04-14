namespace TeoTests.Core;

public abstract class TestBuilder
{
    private readonly HttpClient _httpClient;
    private readonly List<Func<Task<HttpResponseMessage>>> _steps = [];

    protected TestBuilder()
    {
        var appFactory = new AppFactory();
        _httpClient = appFactory.CreateClient();
    }

    protected void With(Func<HttpClient, Task<HttpResponseMessage>> step) =>
        _steps.Add(() => step(_httpClient));

    public async Task<List<HttpResponseMessage>> Build()
    {
        var result = new List<HttpResponseMessage>();

        foreach (var step in _steps)
            result.Add(await step());

        return result;
    }
}