using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.Extensions;
using NTeoTestBuildeR.Modules.Stats.Api;
using TeoTests.Core;
using TeoTests.Modules.TodosModule.Builder;

namespace TeoTests.Modules.StatsModule.Builder;

internal sealed class StatsTestBuilder : TestBuilder<StatsTestBuilder>
{
    internal StatsTestBuilder AddStats(string description, string? tag)
    {
        With(async () =>
        {
            var requestPayload = new AddStats.Request(tag!);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri: "/stats");
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await SendAsync(httpRequest);
            return await httpResponse.Deserialize(description, httpRequest);
        });
        return this;
    }

    internal StatsTestBuilder GetStats(string description, string[] tags)
    {
        With(async () =>
        {
            var query = new QueryBuilder();
            foreach (var tag in tags)
                query.Add(key: "tags", tag);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri: $"/stats{query.ToQueryString()}");
            var httpResponse = await SendAsync(httpRequest);
            return await httpResponse.Deserialize(description, httpRequest);
        });
        return this;
    }

    internal StatsTestBuilder DeleteStats(string description, string tag)
    {
        With(async () =>
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, requestUri: $"/stats/{tag}");
            var httpResponse = await SendAsync(httpRequest);
            return await httpResponse.Deserialize(description, httpRequest);
        });
        return this;
    }
}