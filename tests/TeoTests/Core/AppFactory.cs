using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NTeoTestBuildeR;
using Testcontainers.PostgreSql;
using WireMock.Server;
using WireMock.Settings;

namespace TeoTests.Core;

public sealed class AppFactory : WebApplicationFactory<Program>
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
        .ConfigureAppConfiguration(ConfigureTestcontainers)
        .ConfigureAppConfiguration(ConfigureWiremock);

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