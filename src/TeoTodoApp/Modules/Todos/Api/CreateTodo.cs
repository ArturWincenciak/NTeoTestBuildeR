using JetBrains.Annotations;

namespace NTeoTestBuildeR.Modules.Todos.Api;

[PublicAPI]
public record CreateTodo(CreateTodo.Request Dto)
{
    [PublicAPI]
    public record Request(string Title, string[] Tags);

    [PublicAPI]
    public record Response(Guid Id);
}