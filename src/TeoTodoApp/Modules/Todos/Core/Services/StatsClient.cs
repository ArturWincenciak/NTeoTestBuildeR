using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.Extensions;
using NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Services;

public sealed class StatsClient(HttpClient httpClient)
{
    public async Task AddStats(AddStatsRequest stats)
    {
        var request = new AddStatsRequest(stats.Tag);
        var response = await httpClient.PostAsJsonAsync(requestUri: "/stats", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetStatsResponse> GetStats(string[] tags)
    {
        var query = new QueryBuilder();
        foreach (var tag in tags)
            query.Add(key: "tags", tag);

        var httpResponse = await httpClient.GetAsync($"/stats{query.ToQueryString()}");
        var result = await httpResponse.Content.ReadFromJsonAsync<GetStatsResponse>();
        return result ?? throw new GetStatsException("Failed to get stats");
    }

    public async Task RemoveStats(string tag)
    {
        var response = await httpClient.DeleteAsync($"/stats/{tag}");
        response.EnsureSuccessStatusCode();
    }

    [PublicAPI]
    public sealed record AddStatsRequest(string Tag);

    [PublicAPI]
    public sealed record GetStatsResponse(GetStatsResponse.Item[] Stats)
    {
        [PublicAPI]
        public sealed record Item(string Tag, int Count);
    }
}