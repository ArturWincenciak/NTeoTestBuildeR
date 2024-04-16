using TeoTests.Modules.TodosModule.Builder;

namespace TeoTests.Modules.TodosModule;

public class TodosWithCalendarTestsV1
{
    [Fact]
    public async Task GetCalendarTodos()
    {
        // act
        var actual = await new TodosTestBuilder()
            .GetTodos(description: "Retrieve items from the calendar", tags: ["calendar-event"])
            .Build();

        // assert
        await Verify(actual);
    }
}