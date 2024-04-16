using Microsoft.EntityFrameworkCore;
using NTeoTestBuildeR.Modules.Todos.Core.Model;

namespace NTeoTestBuildeR.Modules.Todos.Core.DAL;

public sealed class TeoAppDbContext(DbContextOptions<TeoAppDbContext> options) : DbContext(options)
{
    public required DbSet<Todo> Todos { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("todos");

        modelBuilder
            .Entity<Todo>()
            .HasKey(todo => todo.Id);

        modelBuilder.Entity<Todo>()
            .OwnsOne(navigationExpression: todo => todo.Tags,
                buildAction: ownedNavigationBuilder => ownedNavigationBuilder.ToJson());
    }
}