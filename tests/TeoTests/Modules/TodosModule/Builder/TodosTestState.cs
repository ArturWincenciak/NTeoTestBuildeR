namespace TeoTests.Modules.TodosModule.Builder;

internal sealed class TodosTestState
{
    private Dictionary<string, Todo> Todos { get; } = new();

    public void Upsert(string id, string title, string[] tags, bool done) =>
        Todos[id] = new(id, title, tags, done);

    public Todo SelectByTitle(string withTitle) =>
        Todos.Single(todo =>
            todo.Value.Title != null && todo.Value.Title.Contains(withTitle)).Value;

    internal sealed record Todo(string? Id, string? Title = null, string[]? Tags = null, bool? Done = null);
}