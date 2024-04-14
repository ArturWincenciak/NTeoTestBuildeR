using JetBrains.Annotations;

namespace NTeoTestBuildeR.Modules.Todos.Api;

[PublicAPI]
public record UpdateTodo(Guid Id, UpdateTodo.Request Dto)
{
    [PublicAPI]
    public record Request(string Title, string[] Tags, bool Done);
}