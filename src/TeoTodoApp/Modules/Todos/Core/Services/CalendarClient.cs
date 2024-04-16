using JetBrains.Annotations;
using NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Services;

public sealed class CalendarClient(HttpClient httpClient)
{
    public async Task<EventItemResponse> CreateEvent(string name, DateTime when)
    {
        var request = new EventRequest(name, Type: "todo-list", when);
        var httpResponse = await httpClient.PostAsJsonAsync(requestUri: "/calendar/events", request);
        var result = await httpResponse.Content.ReadFromJsonAsync<EventItemResponse>();
        return result!.Id == Guid.Empty
            ? throw new CreateCalendarEventException("Failed to create calendar event")
            : result;
    }

    public async Task<EventItemResponse[]> GetEvents()
    {
        var httpResponse = await httpClient.GetAsync("/calendar/events?type=todo-list");
        var result = await httpResponse.Content.ReadFromJsonAsync<EventItemResponse[]>();
        return result ?? [];
    }

    public async Task<EventInstanceResponse?> GetEvent(Guid id)
    {
        var httpResponse = await httpClient.GetAsync($"/calendar/events/{id}");
        if (!httpResponse.IsSuccessStatusCode)
            return null;

        var result = await httpResponse.Content.ReadFromJsonAsync<EventInstanceResponse>();
        return result;
    }

    [PublicAPI]
    public sealed record EventRequest(string Name, string Type, DateTime When);

    [PublicAPI]
    public sealed record EventItemResponse(Guid Id, string Name, string Type, DateTime When);

    [PublicAPI]
    public sealed record EventInstanceResponse(string Name, string Type, DateTime When);
}