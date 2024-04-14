using NTeoTestBuildeR.Modules.Todos.Api;
using NTeoTestBuildeR.Modules.Todos.Core.Exceptions;
using NTeoTestBuildeR.Modules.Todos.Core.Model;

namespace NTeoTestBuildeR.Modules.Todos.Core.Services;

public sealed class TodosService
{
    private readonly static Dictionary<Guid, Todo> Todos = new();

    public CreateTodo.Response Create(CreateTodo cmd)
    {
        var id = Guid.NewGuid();
        Todos.Add(id, value: new()
        {
            Id = id,
            Title = cmd.Dto.Title,
            Done = false,
            Tags = new() {Tags = cmd.Dto.Tags}
        });

        return new(id);
    }

    public void Update(UpdateTodo cmd)
    {
        if (!Todos.TryGetValue(cmd.Id, value: out var todo))
            throw new TodoNotFoundException($"Todo with ID {cmd.Id} not found");

        if (todo.Done)
            throw new TodoAlreadyDoneException("Cannot update a todo that is already done");

        Todos[todo.Id] = new()
        {
            Id = todo.Id,
            Title = cmd.Dto.Title,
            Done = cmd.Dto.Done,
            Tags = new() {Tags = cmd.Dto.Tags}
        };
    }

    public GetTodo.Response GetTodo(GetTodo query)
    {
        if (!Todos.TryGetValue(query.Dto.Id, value: out var todo))
            throw new TodoNotFoundException($"Todo with ID {query.Dto.Id} not found");

        return new(todo.Title, todo.Tags.Tags, todo.Done);
    }

    public GetTodos.Response GetTodos(GetTodos.Query query) =>
        new(Todos.Values
            .Where(todo => query.Tags.All(queryTag => todo.Tags.Tags.Contains(queryTag)))
            .Select(todo => new GetTodos.Item(todo.Id, todo.Title, Done: todo.Done, Tags: todo.Tags.Tags
                .Select(tag => new GetTodos.Tag(tag, Count: null))
                .ToArray()))
            .ToArray());
}