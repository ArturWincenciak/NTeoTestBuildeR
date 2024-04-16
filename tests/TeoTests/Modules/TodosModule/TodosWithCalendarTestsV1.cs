using System.Net;
using System.Text.Json;
using NTeoTestBuildeR.Modules.Todos.Core.Services;
using TeoTests.Modules.TodosModule.Builder;
using WireMock.Matchers;
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
            .WithWiremock(configure: GetCalendarEventsReturnsFewItems(), expectedCallCount: 1)
            .GetTodos(description: "Retrieve items from the calendar", tags: ["calendar-event"])
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task GetEmptyCalendarTodos()
    {
        // act
        var actual = await new TodosTestBuilder()
            .WithWiremock(GetCalendarEventsReturnsZeroItems())
            .GetTodos(description: "Retrieve items from the calendar", tags: ["calendar-event"])
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task CreateCalendarTodo()
    {
        // arrange
        var eventName = "Daily stand up";
        var calendarType = "todo-list";
        var when = UtcNow.AddHours(24);

        // act
        var actual = await new TodosTestBuilder()
            .WithWiremock(AddCalendarEventWithCreatedStatus(eventName, calendarType, when))
            .CreateTodo(description: "Create a to-do item in the calendar",
                eventName, tags: ["calendar-event", $"{when:O}"])
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task CreateCalendarTodoWentWrong()
    {
        // arrange
        var eventName = "Conf-call with some company pets";
        var calendarType = "todo-list";
        var when = UtcNow.AddHours(1);

        // act
        var actual = await new TodosTestBuilder()
            .WithWiremock(AddCalendarEventReturnsEmptyId(eventName, calendarType, when))
            .CreateTodo(description: "Create a to-do item in the calendar that went wrong due to empty id",
                eventName, tags: ["calendar-event", $"{when:O}"])
            .Build();

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

    private static Action<(IRequestBuilder request, IResponseBuilder response)> GetCalendarEventsReturnsZeroItems() =>
        server =>
        {
            server.request
                .WithPath("/calendar/events")
                .WithParam(key: "type", "todo-list")
                .UsingGet();

            server.response
                .WithBody(JsonSerializer.Serialize(Array.Empty<CalendarClient.EventItemResponse>()))
                .WithStatusCode(HttpStatusCode.OK);
        };

    private static Action<(IRequestBuilder request, IResponseBuilder response)> AddCalendarEventWithCreatedStatus(
        string eventName, string calendarType, DateTime when) =>
        server =>
        {
            server.request
                .WithPath("/calendar/events")
                .UsingPost()
                .WithBody(new ExactMatcher(
                    ignoreCase: true, JsonSerializer.Serialize(
                        new CalendarClient.EventRequest(eventName, calendarType, when))));

            server.response
                .WithBody(JsonSerializer.Serialize(
                    new CalendarClient.EventItemResponse(Id: Guid.NewGuid(), eventName, calendarType, when)))
                .WithStatusCode(HttpStatusCode.Created);
        };

    private static Action<(IRequestBuilder request, IResponseBuilder response)> AddCalendarEventReturnsEmptyId(
        string eventName, string calendarType, DateTime when) =>
        server =>
        {
            server.request
                .WithPath("/calendar/events")
                .UsingPost()
                .WithBody(new ExactMatcher(
                    ignoreCase: true, JsonSerializer.Serialize(
                        new CalendarClient.EventRequest(eventName, calendarType, when))));

            server.response
                .WithBody(JsonSerializer.Serialize(
                    new CalendarClient.EventItemResponse(Guid.Empty, eventName, calendarType, when)))
                .WithStatusCode(HttpStatusCode.Created);
        };
}