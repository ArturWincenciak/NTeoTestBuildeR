using ExtCalendar;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var calendar = new Dictionary<Guid, (Guid Id, string Name, string Type, DateTime When)>();

app.MapPost(pattern: "/calendar/events", handler: ([FromBody] SetupEventRequest @event) =>
{
    var id = Guid.NewGuid();
    calendar.Add(id, value: new ValueTuple<Guid, string, string, DateTime>(id, @event.Name, @event.Type, @event.When));
    return Results.Created(uri: $"/calendar/events/{id}", value: new {id, @event.Name, @event.Type, @event.When});
}).WithOpenApi();

app.MapGet(pattern: "/calendar/events/{id:guid}", handler: ([FromRoute] Guid id) =>
    calendar.TryGetValue(id, value: out var @event)
        ? Results.Ok(new {@event.Name, @event.Type, @event.When})
        : Results.NotFound())
    .WithOpenApi();

app.MapGet(pattern: "/calendar/events", handler: ([FromQuery] string type) =>
    calendar.Values
        .Where(@event => @event.Type == type)
        .Select(@event => new {@event.Id, @event.Name, @event.Type, @event.When}))
    .WithOpenApi();

app.Run();

namespace ExtCalendar
{
[PublicAPI]
internal sealed record SetupEventRequest(string Name, string Type, DateTime When);
}