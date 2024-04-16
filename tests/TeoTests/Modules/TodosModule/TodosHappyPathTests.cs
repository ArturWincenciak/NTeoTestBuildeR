using TeoTests.Modules.TodosModule.Builder;

namespace TeoTests.Modules.TodosModule;

public class TodosHappyPathTests
{
    [Fact]
    public async Task CreateTodo()
    {
        // act
        var testCase = "1D1E4128-31F3-4108-8CF6-C2E7F2E495BC";
        var title = $"Prove Riemann's hypothesis {testCase}";
        var tag = "match";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Create a to-do item with success", title, tags: [tag])
            .GetTodo(description: "Get already created to-do item", title)
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task DoneTodo()
    {
        // assert
        var testCase = "1D1E4128-31F3-4108-8CF6-C2E7F2E495BC";
        var title = $"Land on the moon {testCase}";
        var tag = "astronomy";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Set up a to-do", title, tags: [tag])
            .GetTodo(description: "Retrieve already created to-do item", title)
            .DoneTodo(description: "Mark the to-do as done", title)
            .GetTodo(description: "Retrieve the to-do that has been done", title)
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task ChangeTodoTitle()
    {
        // arrange 
        var testCase = "FB4CBDC4-2E75-4B31-AD97-9AF922A6D24C";
        var title = $"Land on the Mars {testCase}";
        var newTitle = $"Terraform Mars {testCase}";
        var tag = "astronomy";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Set up a to-do", title, tags: [tag])
            .GetTodo(description: "Retrieve already created to-do item", title)
            .ChangeTitle(description: "Change title from 'Land' to 'Terraform'", title, newTitle)
            .GetTodo(description: "Retrieve the to-do that has been changed", newTitle)
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task ChangeTodoTags()
    {
        // arrange 
        var testCase = "B1589CC6-5B47-428A-BF14-3A7B10D09B8B";
        var title = $"Land on the Mars {testCase}";
        string[] tags = ["astronomy"];
        string[] newTags = ["astronomy", "practical"];

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Set up a to-do", title, tags)
            .GetTodo(description: "Retrieve already created to-do item", title)
            .ChangeTags(description: "Change tags by adding one more tag", title, newTags)
            .GetTodo(description: "Retrieve the to-do that has been changed", title)
            .Build();

        // assert
        await Verify(actual);
    }

    [Fact]
    public async Task ChangeTagsAndMarkTodoAsDone()
    {
        // arrange
        var testCase = "A052551C-4577-4D84-9FFC-AA7227F11C54";
        var theoryTitle = $"Define theory of everything {testCase}";
        var flightTitle = $"Flight to Alpha Centauri {testCase}";
        var astronomy = "astronomy";
        var physics = "physics";
        var theory = "theoretical";
        var practice = "practical";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Set up first theoretical to-do", theoryTitle, tags: [astronomy])
            .CreateTodo(description: "Set up second practical to-do", flightTitle, tags: [astronomy])
            .ChangeTags(description: "Change tags of the theory", theoryTitle, newTags: [physics, theory])
            .ChangeTags(description: "Change tags of the practice", flightTitle, newTags: [astronomy, practice])
            .DoneTodo(description: "Mark the theoretical to-do as done", theoryTitle)
            .GetTodo(description: "Retrieve the theoretical to-do that has been done", theoryTitle)
            .GetTodo(description: "Retrieve the practical that has not been done yet", flightTitle)
            .Build();

        // assert
        await Verify(target: actual);
    }

    [Fact]
    public async Task GetTodosByTagsInQueryString()
    {
        // arrange
        var @case = "A658D7F2-5298-4145-A22F-404F71B10570";
        var match = $"match-{@case}";
        var physics = $"physics-{@case}";
        var theory = $"theoretical-{@case}";
        var exp = $"experimental-{@case}";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(
                description: "Set up a math and theoretical",
                title: "Prove Riemann's hypothesis", tags: [match, theory])
            .CreateTodo(
                description: "Set up a physics and theoretical",
                title: "Define theory of everything", tags: [physics, theory])
            .CreateTodo(
                description: "Set up a physics and experimental",
                title: "Conduct a measurement of the gravitational wave", tags: [physics, exp])
            .GetTodos(
                description: "Should only retrieve one item with math", tags: [match])
            .GetTodos(
                description: "Should retrieve two items with physics", tags: [physics])
            .GetTodos(
                description: "Should retrieve two items with theoretical", tags: [theory])
            .GetTodos(
                description: "Should retrieve one item with experimental", tags: [exp])
            .GetTodos(
                description: "Should retrieve one item with both math and theoretical", tags: [match, theory])
            .GetTodos(
                description: "Should retrieve one item with both physics and theoretical", tags: [physics, theory])
            .GetTodos(
                description: "Should retrieve one item with both physics and experimental", tags: [physics, exp])
            .Build();

        // assert
        await Verify(target: actual);
    }
}