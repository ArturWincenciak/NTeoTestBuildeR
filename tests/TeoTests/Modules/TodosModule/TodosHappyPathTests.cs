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

    [Fact]
    public async Task DoneTodo()
    {
        // assert
        var title = "Land on the moon";
        var tag = "astronomy";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Set up a to-do", title, tags: [tag])
            .GetTodo(description: "Retrieve already created to-do item")
            .DoneTodo(description: "Mark the to-do as done")
            .GetTodo(description: "Retrieve the to-do that has been done")
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task ChangeTodoTitle()
    {
        // arrange 
        var title = "Land on the Mars";
        var newTitle = "Terraform Mars";
        var tag = "astronomy";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Set up a to-do", title, tags: [tag])
            .GetTodo(description: "Retrieve already created to-do item")
            .ChangeTitle(description: "Change title from 'Land' to 'Terraform'", newTitle)
            .GetTodo(description: "Retrieve the to-do that has been changed")
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task ChangeTodoTags()
    {
        // arrange 
        var title = "Land on the Mars";
        string[] tags = ["astronomy"];
        string[] newTags = ["astronomy", "practical"];

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Set up a to-do", title, tags)
            .GetTodo(description: "Retrieve already created to-do item")
            .ChangeTags(description: "Change tags by adding one more tag", newTags)
            .GetTodo(description: "Retrieve the to-do that has been changed")
            .Build();

        // assert
        await Verify(actual);
    }
}