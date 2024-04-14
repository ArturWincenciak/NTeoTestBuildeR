using JetBrains.Annotations;

namespace NTeoTestBuildeR.Modules.Todos.Api;

[PublicAPI]
public record GetTodo(GetTodo.Request Dto)
{
    [PublicAPI]
    public record Request(Guid Id);

    [PublicAPI]
    public record Response(string Title, string[] Tags, bool Done);
}