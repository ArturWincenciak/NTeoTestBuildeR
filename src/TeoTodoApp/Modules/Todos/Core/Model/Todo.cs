namespace NTeoTestBuildeR.Modules.Todos.Core.Model;

public sealed class Todo
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required TagCollection Tags { get; init; }
    public bool Done { get; init; }
}