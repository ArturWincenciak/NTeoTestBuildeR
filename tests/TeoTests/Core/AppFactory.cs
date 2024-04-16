using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NTeoTestBuildeR;
using NTeoTestBuildeR.Modules.Todos.Core.Services;
using Testcontainers.PostgreSql;
using WireMock.Server;
using WireMock.Settings;

namespace TeoTests.Core;

public sealed class AppFactory(Func<HttpClient> createHimselfClient) : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgresContainer = BuildPostgres();

    public WireMockServer Wiremock { get; } = WireMockServer.Start(
        new WireMockServerSettings
        {
            StartAdminInterface = true,
            HandleRequestsSynchronously = true
        });

    private static PostgreSqlContainer BuildPostgres() =>
        new PostgreSqlBuilder()
            .WithImage("postgres")
            .WithDatabase("teo-app")
            .WithAutoRemove(autoRemove: true)
            .WithCleanUp(cleanUp: true)
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder
        .ConfigureServices(services => OverrideStatsClient(services, createHimselfClient))
        .ConfigureAppConfiguration(ConfigureTestcontainers)
        .ConfigureAppConfiguration(ConfigureWiremock);

    private static void OverrideStatsClient(IServiceCollection services, Func<HttpClient> create) =>
        services.AddTransient<StatsClient>(_ =>
            new(create()));

    private void ConfigureTestcontainers(IConfigurationBuilder configurationBuilder)
    {
        _postgresContainer.StartAsync().Wait();
        var connectionString = _postgresContainer.GetConnectionString();

        configurationBuilder.AddInMemoryCollection(new KeyValuePair<string, string?>[]
        {
            new(key: "ConnectionStrings:TeoTodoApp", connectionString)
        });
    }

    private void ConfigureWiremock(IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddInMemoryCollection(new KeyValuePair<string, string?>[]
        {
            new(key: "ExtCalendar:BaseAddress", value: Wiremock.Urls[0])
        });
    }
}