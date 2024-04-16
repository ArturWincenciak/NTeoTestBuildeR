using Microsoft.EntityFrameworkCore;
using NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;
using NTeoTestBuildeR.Modules.Todos.Api;
using NTeoTestBuildeR.Modules.Todos.Core.DAL;
using NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Services;

public sealed class TodosService(TeoAppDbContext db)
{
    public async Task<CreateTodo.Response> Create(CreateTodo cmd)
    {
        ValidateArgument(detail: "Invalid argument for creating a new todo", with: exception =>
        {
            if (string.IsNullOrWhiteSpace(cmd.Dto.Title))
                exception.WithError(code: $"{nameof(cmd.Dto.Title)}",
                    values: ["Title is required, cannot be empty or white spaces"]);

            if (cmd.Dto.Tags.Length == 0)
                exception.WithError(code: $"{nameof(cmd.Dto.Tags)}",
                    values: ["At least one tag is required"]);

            if (cmd.Dto.Tags.Any(tag => string.IsNullOrWhiteSpace(tag) || tag.Contains(' ')))
                exception.WithError(code: $"{nameof(cmd.Dto.Tags)}",
                    values: ["Tags cannot be empty or contain spaces"]);
        });

        var id = Guid.NewGuid();
        await db.Todos.AddAsync(new()
        {
            Id = id,
            Title = cmd.Dto.Title,
            Tags = new() {Tags = cmd.Dto.Tags},
            Done = false
        });
        await db.SaveChangesAsync();

        return new(id);
    }

    public async Task Update(UpdateTodo cmd)
    {
        ValidateArgument(detail: "Invalid argument for updating existing todo", with: exception =>
        {
            if (string.IsNullOrWhiteSpace(cmd.Dto.Title))
                exception.WithError(code: $"{nameof(cmd.Dto.Title)}",
                    values: ["Title is required, cannot be empty or white spaces"]);

            if (cmd.Dto.Tags.Length == 0)
                exception.WithError(code: $"{nameof(cmd.Dto.Tags)}",
                    values: ["At least one tag is required"]);

            if (cmd.Dto.Tags.Any(tag => string.IsNullOrWhiteSpace(tag) || tag.Contains(' ')))
                exception.WithError(code: $"{nameof(cmd.Dto.Tags)}",
                    values: ["Tags cannot be empty or contain spaces"]);
        });

        var todo = await db.Todos
            .SingleOrDefaultAsync(item => item.Id == cmd.Id);

        if (todo is null)
            throw new TodoNotFoundException($"Todo with ID {cmd.Id} not found");

        if (todo.Done)
            throw new TodoAlreadyDoneException("Cannot update a todo that is already done");

        todo.Title = cmd.Dto.Title;
        todo.Tags = new() {Tags = cmd.Dto.Tags};
        todo.Done = cmd.Dto.Done;

        await db.SaveChangesAsync();
    }

    public async Task<GetTodo.Response> GetTodo(GetTodo query)
    {
        var todo = await db.Todos
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == query.Dto.Id);

        if (todo is null)
            throw new TodoNotFoundException($"Todo with ID {query.Dto.Id} not found");
        
        return new(todo.Title, todo.Tags.Tags, todo.Done);
    }

    public async Task<GetTodos.Response> GetTodos(GetTodos.Query query)
    {
        var todos = await db.Todos
            .AsNoTracking()
            .Where(todo => query.Tags.All(queryTag => todo.Tags.Tags.Contains(queryTag)))
            .ToListAsync();

        return new(todos
            .Select(todo => new GetTodos.Item(todo.Id, todo.Title, Done: todo.Done, Tags: todo.Tags.Tags
                .Select(tag => new GetTodos.Tag(tag, Count: null))
                .ToArray()))
            .ToArray());
    }

    private static void ValidateArgument(string detail, Action<AppArgumentException> with)
    {
        var exception = new InvalidTodoAppArgumentException(detail);

        with(exception);

        if (exception.HasErrors)
            throw exception;
    }
}