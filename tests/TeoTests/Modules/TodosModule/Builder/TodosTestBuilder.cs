using System.Net.Http.Json;
using NTeoTestBuildeR.Modules.Todos.Api;
using TeoTests.Core;

namespace TeoTests.Modules.TodosModule.Builder;

internal sealed class TodosTestBuilder : TestBuilder
{
    private Uri? _todoLocation;

    internal TodosTestBuilder CreateTodo(string? title, string?[]? tags)
    {
        With(async httpClient =>
        {
            var requestPayload = new CreateTodo.Request(Title: title!, Tags: tags!);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri: "/todos");
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);
            _todoLocation = httpResponse.Headers.Location;
            return httpResponse;
        });
        return this;
    }

    internal TodosTestBuilder GetTodo()
    {
        With(async httpClient =>
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, _todoLocation);
            return await httpClient.SendAsync(httpRequest);
        });
        return this;
    }
}