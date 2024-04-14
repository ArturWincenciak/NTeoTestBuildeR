using JetBrains.Annotations;

namespace NTeoTestBuildeR.Modules.Todos.Api;

[PublicAPI]
public record GetTodos
{
    [PublicAPI]
    public record Query(string[] Tags);

    [PublicAPI]
    public record Response(Item[] Todos);

    [PublicAPI]
    public record Item(Guid Id, string Title, Tag[] Tags, bool Done);

    public record Tag(string Name, int? Count);
}