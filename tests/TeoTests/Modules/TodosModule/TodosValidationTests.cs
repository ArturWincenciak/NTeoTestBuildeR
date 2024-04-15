using TeoTests.Modules.TodosModule.Builder;

namespace TeoTests.Modules.TodosModule;

public class TodosValidationTests
{
    [Fact]
    public async Task CreateTodo()
    {
        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Should not create with whitespace title", title: "    ", tags: ["tag"])
            .CreateTodo(description: "Should not create without any tags", title: "Title", tags: [])
            .CreateTodo(description: "Should not create without title and tags", title: "", tags: [])
            .CreateTodo(description: "Should not create with whitespace tag", title: "Title", tags: ["tag with space"])
            .Build();

        // assert
        await Verify(target: actual);
    }

    [Fact]
    public async Task UpdateTodo()
    {
        // arrange
        var testCase = "C49EDFB3-1DAE-4969-9FAC-D4B5E08A7B53";
        var validTitle = $"Go to the moon {testCase}";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Create valid to-do for updating test cases", validTitle, tags: ["astronomy"])
            .ChangeTitle(description: "Should not update with empty title", validTitle, newTitle: "")
            .ChangeTitle(description: "Should not update with whitespace title", validTitle, newTitle: "    ")
            .ChangeTags(description: "Should not update with empty tags", validTitle, newTags: [])
            .ChangeTags(description: "Should not update with whitespace tag", validTitle, newTags: ["tag with space"])
            .Build();

        // assert
        await Verify(target: actual);
    }

    [Fact]
    public async Task ChangeTitleForCompletedTodo()
    {
        // arrange
        var testCase = "CCC650C6-105F-4D82-B470-D0AA85B104C2";
        var title = $"Calculate the speed of light {testCase}";
        var newTitle = $"Define theory of everything {testCase}";
        var tag = "physics";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(description: "Set up a to-do", title, tags: [tag])
            .GetTodo(description: "Retrieve already created to-do item", title)
            .DoneTodo(description: "Mark the to-do as done", title)
            .ChangeTitle(description: "Try to change the title", title, newTitle)
            .Build();

        // assert
        await Verify(target: actual);
    }
}