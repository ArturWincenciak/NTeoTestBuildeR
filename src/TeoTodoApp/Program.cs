using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using NTeoTestBuildeR.Infra.ErrorHandling;
using NTeoTestBuildeR.Modules.Todos.Core.DAL;
using NTeoTestBuildeR.Modules.Todos.Core.Exceptions;
using NTeoTestBuildeR.Modules.Todos.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddScoped<ErrorHandlerMiddleware>()
    .AddScoped<TodosService>()
    .AddControllers()
    .Services.AddSwaggerGen(options => options
        .CustomSchemaIds(type => type.FullName?
            .Replace(oldValue: "+", string.Empty)))
    .AddDbContext<TeoAppDbContext>((serviceProvider, options) => options.UseNpgsql(
        connectionString: serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("TeoTodoApp"),
        npgsqlOptionsAction: npgsqlOptionsBuilder => npgsqlOptionsBuilder.EnableRetryOnFailure()))
    .AddScoped<CalendarClient>()
    .AddHttpClient<CalendarClient>((serviceProvider, client) => client.BaseAddress = new(
        serviceProvider.GetRequiredService<IConfiguration>()
            .GetSection("ExtCalendar:BaseAddress")
            .Get<string>() ??
        throw new CalendarClientUriException()))
    .Services.AddMemoryCache()
    .AddHttpClient<StatsClient>((serviceProvider, client) => client.BaseAddress = new(
        serviceProvider.GetRequiredService<IConfiguration>()
            .GetSection("StatsModule:BaseAddress")
            .Get<string>() ??
        throw new StatsClientUriException()));

var application = builder.Build();
application
    .UseSwagger()
    .UseSwaggerUI()
    .UseRouting()
    .UseMiddleware<ErrorHandlerMiddleware>();

using (var scope = application.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TeoAppDbContext>();
    if (db.Database.GetPendingMigrations().Any())
        db.Database.Migrate();
}

application.MapControllers();
application.Run();

namespace NTeoTestBuildeR
{
[UsedImplicitly]
public class Program;
}