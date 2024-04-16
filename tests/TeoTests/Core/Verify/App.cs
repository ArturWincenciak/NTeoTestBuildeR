using Microsoft.AspNetCore.Hosting;

namespace TeoTests.Core.Verify;

public sealed class App
{
    private readonly static Lazy<App> LazyInstance = new(() =>
    {
        var appFactory = new AppFactory();
        var appBuilder = appFactory
            .WithWebHostBuilder(builder => builder
                .UseContentRoot(Directory.GetCurrentDirectory()));

        var httpClient = appBuilder.CreateClient();
        return new(httpClient);
    });

    private volatile HttpClient _httpClient;
    public static HttpClient HttpClient => LazyInstance.Value._httpClient;

    private App(HttpClient httpClient) =>
        _httpClient = httpClient;
}