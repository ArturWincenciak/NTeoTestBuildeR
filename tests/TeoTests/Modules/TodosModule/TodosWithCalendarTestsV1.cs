using System.Net;
using System.Text.Json;
using NTeoTestBuildeR.Modules.Todos.Core.Services;
using TeoTests.Core.Verify;
using TeoTests.Modules.TodosModule.Builder;
using WireMock.Net.Extensions.WireMockInspector;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static System.DateTime;

namespace TeoTests.Modules.TodosModule;

public class TodosWithCalendarTestsV1
{
    [Fact]
    public async Task GetCalendarTodos()
    {
        // act
        var actual = await new TodosTestBuilder()
            .WithWiremock(GetCalendarEventsReturnsFewItems())
            .GetTodos(description: "Retrieve items from the calendar", tags: ["calendar-event"])
            .Build();

        // App.Wiremock.Inspect();

        // assert
        await Verify(actual);
    }

    private static Action<(IRequestBuilder request, IResponseBuilder response)> GetCalendarEventsReturnsFewItems() =>
        server =>
        {
            server.request
                .WithPath("/calendar/events")
                .WithParam(key: "type", "todo-list")
                .UsingGet();

            server.response
                .WithBody(JsonSerializer.Serialize(new CalendarClient.EventItemResponse[]
                {
                    new(Id: Guid.NewGuid(), Name: "Daily stand up", Type: "todo-list", When: UtcNow.AddHours(-1)),
                    new(Id: Guid.NewGuid(), Name: "Lunch", Type: "todo-list", When: UtcNow.AddMinutes(30)),
                    new(Id: Guid.NewGuid(), Name: "Meeting", Type: "todo-list", When: UtcNow.AddHours(4)),
                    new(Id: Guid.NewGuid(), Name: "Sprint review", Type: "todo-list", When: UtcNow.AddDays(2)),
                    new(Id: Guid.NewGuid(), Name: "Finish app", Type: "todo-list", When: UtcNow.AddDays(120))
                }))
                .WithStatusCode(HttpStatusCode.OK);
        };
}