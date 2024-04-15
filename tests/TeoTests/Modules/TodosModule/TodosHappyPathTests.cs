using TeoTests.Modules.TodosModule.Builder;

namespace TeoTests.Modules.TodosModule;

public class TodosHappyPathTests
{
    [Fact]
    public async Task CreateTodo()
    {
        // act
        var title = "Prove Riemann's hypothesis";
        var tag = "match";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Create a to-do item with success", title, tags: [tag])
            .GetTodo(description: "Get already created to-do item")
            .Build();

        // assert
        await Verify(actual);
    }
}