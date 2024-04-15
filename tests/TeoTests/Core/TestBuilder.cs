namespace TeoTests.Core;

public abstract class TestBuilder
{
    private readonly HttpClient _httpClient;
    private readonly List<Func<Task<object?>>> _steps = [];

    protected TestBuilder()
    {
        var appFactory = new AppFactory();
        _httpClient = appFactory.CreateClient();
    }

    protected void With(Func<HttpClient, Task<object?>> step) =>
        _steps.Add(() => step(_httpClient));

    public async Task<List<object?>> Build()
    {
        var result = new List<object?>();

        foreach (var step in _steps)
            result.Add(await step());

        return result;
    }
}