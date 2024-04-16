using System.Net;
using System.Text.Json;
using NTeoTestBuildeR.Modules.Todos.Core.Services;
using TeoTests.Modules.TodosModule.Builder;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace TeoTests.Modules.TodosModule;

public class TodosWithCalendarTestsV2
{
    [Fact]
    public async Task GetUniqueCalendarTodo()
    {
        // arrange
        var calendarEventId = Guid.Parse("f74ec649-0212-481c-b058-a4e1651c79fb");

        // act
        var actual = await new TodosTestBuilder()
            .WithWiremock(configure: GetCalendarEvent(calendarEventId), expectedCallCount: 1)
            .GetTodo(description: "Retrieve a single to-do item from the calendar", calendarEventId)
            .GetTodo(description: "Second attempt to retrieve the same to-do item", calendarEventId)
            .Build();

        // assert
        await Verify(actual);
    }

    private static Action<(IRequestBuilder request, IResponseBuilder response)> GetCalendarEvent(
        Guid calendarEventId) => server =>
    {
        server.request
            .WithPath($"/calendar/events/{calendarEventId}")
            .UsingGet();

        server.response
            .WithBody(JsonSerializer.Serialize(new CalendarClient.EventInstanceResponse(
                Name: "Sort out life", Type: "todo-list", When: DateTime.UtcNow.AddYears(10))))
            .WithStatusCode(HttpStatusCode.OK);
    };
}