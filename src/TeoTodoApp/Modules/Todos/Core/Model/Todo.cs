namespace NTeoTestBuildeR.Modules.Todos.Core.Model;

public sealed class Todo
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required TagCollection Tags { get; set; }
    public bool Done { get; set; }
}