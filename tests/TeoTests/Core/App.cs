using Microsoft.AspNetCore.Hosting;
using WireMock.Server;

namespace TeoTests.Core;

public sealed class App
{
    private readonly static Lazy<App> LazyInstance = new(() =>
    {
        var appFactory = new AppFactory(() => HttpClient);
        var appBuilder = appFactory
            .WithWebHostBuilder(builder => builder
                .UseContentRoot(Directory.GetCurrentDirectory()));

        var httpClient = appBuilder.CreateClient();
        return new(httpClient, appFactory);
    });

    private volatile AppFactory _appFactory;
    private volatile HttpClient _httpClient;
    public static HttpClient HttpClient => LazyInstance.Value._httpClient;
    public static WireMockServer Wiremock => LazyInstance.Value._appFactory.Wiremock;

    private App(HttpClient httpClient, AppFactory appFactory)
    {
        _httpClient = httpClient;
        _appFactory = appFactory;
    }
}