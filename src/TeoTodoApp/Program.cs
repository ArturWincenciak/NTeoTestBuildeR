using JetBrains.Annotations;
using NTeoTestBuildeR.Infra.ErrorHandling;
using NTeoTestBuildeR.Modules.Todos.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddScoped<ErrorHandlerMiddleware>()
    .AddScoped<TodosService>()
    .AddControllers()
    .Services.AddSwaggerGen(options => options
        .CustomSchemaIds(type => type.FullName?
            .Replace(oldValue: "+", string.Empty)));

var application = builder.Build();
application
    .UseSwagger()
    .UseSwaggerUI()
    .UseRouting()
    .UseMiddleware<ErrorHandlerMiddleware>();

application.MapControllers();
application.Run();

namespace NTeoTestBuildeR
{
[UsedImplicitly]
public class Program;
}