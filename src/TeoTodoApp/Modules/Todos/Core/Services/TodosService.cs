using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;
using NTeoTestBuildeR.Modules.Todos.Api;
using NTeoTestBuildeR.Modules.Todos.Core.DAL;
using NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Services;

public sealed class TodosService(
    TeoAppDbContext db,
    CalendarClient calendarClient,
    IMemoryCache cache,
    StatsClient statsClient)
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

        if (cmd.Dto.Tags.Contains("calendar-event"))
        {
            ValidateArgument(detail: "Invalid additional tag argument for calendar-event tag", with: exception =>
            {
                if (cmd.Dto.Tags.Length != 2)
                    exception.WithError(code: $"{nameof(cmd.Dto.Tags)}", values:
                    [
                        "Only two tags are allowed for calendar events, " +
                        "first tag must be 'calendar-event' and the second tag must be a time tag"
                    ]);
                else if (DateTime.TryParse(s: cmd.Dto.Tags[1], out _) == false)
                    exception.WithError(code: $"{nameof(cmd.Dto.Tags)}", values:
                    [
                        $"Second tag must be a valid date time but is '{cmd.Dto.Tags[1]}'"
                    ]);
            });

            var when = DateTime.Parse(cmd.Dto.Tags[1]).ToUniversalTime();
            var createdEvent = await calendarClient.CreateEvent(cmd.Dto.Title, when);
            return new(createdEvent.Id);
        }

        var id = Guid.NewGuid();
        await db.Todos.AddAsync(new()
        {
            Id = id,
            Title = cmd.Dto.Title,
            Tags = new() {Tags = cmd.Dto.Tags},
            Done = false
        });
        await db.SaveChangesAsync();

        foreach (var dtoTag in cmd.Dto.Tags)
            await statsClient.AddStats(new(dtoTag));

        return new(id);
    }

    public async Task Update(UpdateTodo cmd)
    {
        ValidateArgument(detail: "Invalid argument for updating existing todo", with: exception =>
        {
            if (cmd.Dto.Tags.Contains("calendar-event"))
                exception.WithError(code: $"{nameof(cmd.Dto.Tags)}",
                    values: ["Update calendar events is not not supported, use the calendar API instead"]);

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

        var oldTags = todo.Tags.Tags;
        var newTags = cmd.Dto.Tags;
        todo.Title = cmd.Dto.Title;
        todo.Tags = new() {Tags = newTags};
        todo.Done = cmd.Dto.Done;

        await db.SaveChangesAsync();

        var isOldAndNewTagsEqual = oldTags.Length == newTags.Length && oldTags.All(oldTag => newTags.Contains(oldTag));
        if (!isOldAndNewTagsEqual)
        {
            foreach (var oldTag in oldTags)
                await statsClient.RemoveStats(new(oldTag));

            foreach (var newTag in newTags)
                await statsClient.AddStats(new(newTag));
        }
    }

    public async Task<GetTodo.Response> GetTodo(GetTodo query)
    {
        var todo = await db.Todos
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == query.Dto.Id);

        if (todo is not null)
            return new(todo.Title, todo.Tags.Tags, todo.Done);

        var cachedEvent = cache.Get<GetTodo.Response>(query.Dto.Id);
        if (cachedEvent is not null)
            return cachedEvent;

        var @event = await calendarClient.GetEvent(query.Dto.Id);
        if (@event is null)
            throw new TodoNotFoundException($"Todo with ID {query.Dto.Id} not found");

        var mapped = Map(eventItem: (query.Dto.Id, @event.Name, @event.Type, @event.When), DateTime.UtcNow);
        var result = new GetTodo.Response(mapped.Title, mapped.Tags, mapped.Done);
        return cache.Set(query.Dto.Id, result);
    }

    public async Task<GetTodos.Response> GetTodos(GetTodos.Query query)
    {
        if (query.Tags.Contains("calendar-event") && query.Tags.Length > 1)
            throw new AppArgumentException("Cannot mix calendar events with other to-do items");

        if (query.Tags.Contains("calendar-event"))
        {
            var now = DateTime.UtcNow;

            var events = await calendarClient.GetEvents();
            return new(events
                .Select(@event =>
                {
                    var item = Map(eventItem: (@event.Id, @event.Name, @event.Type, @event.When), now);
                    return new GetTodos.Item(item.Id, item.Title, Done: item.Done,
                        Tags: item.Tags.Select(tag => new GetTodos.Tag(tag, Count: null)).ToArray());
                })
                .ToArray());
        }

        var todos = await db.Todos
            .AsNoTracking()
            .Where(todo => query.Tags.All(queryTag => todo.Tags.Tags.Contains(queryTag)))
            .ToListAsync();

        var uniqueTags = todos
            .SelectMany(todo => todo.Tags.Tags)
            .ToArray();

        var statsTags = await statsClient.GetStats(uniqueTags);

        return new(todos
            .Select(todo =>
            {
                return new GetTodos.Item(todo.Id, todo.Title, Done: todo.Done,
                    Tags: todo.Tags.Tags.Select(tag =>
                    {
                        var stat = statsTags.Stats.Single(stat => stat.Tag == tag);
                        return new GetTodos.Tag(tag, stat.Count);
                    }).ToArray());
            })
            .ToArray());
    }

    private static (Guid Id, string Title, string[] Tags, bool Done) Map(
        (Guid Id, string Name, string Type, DateTime When) eventItem, DateTime now)
    {
        if (eventItem.When < now)
            return (eventItem.Id, eventItem.Name,
                Tags: ["calendar-event"], Done: true);

        if (eventItem.When < now.AddHours(1))
            return (eventItem.Id, eventItem.Name,
                Tags: ["calendar-event", "in-an-hour"], Done: false);

        if (eventItem.When < now.AddHours(24))
            return (eventItem.Id, eventItem.Name,
                Tags: ["calendar-event", "in-a-day"], Done: false);

        if (eventItem.When < now.AddDays(7))
            return (eventItem.Id, eventItem.Name,
                Tags: ["calendar-event", "in-a-week"], Done: false);

        return (eventItem.Id, eventItem.Name,
            Tags: ["calendar-event", "someday"], Done: false);
    }

    private static void ValidateArgument(string detail, Action<AppArgumentException> with)
    {
        var exception = new InvalidTodoAppArgumentException(detail);

        with(exception);

        if (exception.HasErrors)
            throw exception;
    }
}