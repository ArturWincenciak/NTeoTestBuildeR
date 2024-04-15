using System.Net.Http.Json;
using NTeoTestBuildeR.Modules.Todos.Api;
using TeoTests.Core;
using TeoTests.Core.Verify;

namespace TeoTests.Modules.TodosModule.Builder;

internal sealed class TodosTestBuilder : TestBuilder
{
    private Todo? _todo;

    internal TodosTestBuilder CreateTodo(string description, string? title, string?[]? tags)
    {
        With(async httpClient =>
        {
            var requestPayload = new CreateTodo.Request(Title: title!, Tags: tags!);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri: "/todos");
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);
            var responsePayload = await httpResponse.Content.ReadFromJsonAsync<CreateTodo.Response>();
            _todo = new(Id: responsePayload!.Id.ToString(), title, tags, Done: false);

            return new Actual(description,
                Request: Request.Create(httpRequest, requestPayload),
                Response: Response.Create(httpResponse, responsePayload));
        });
        return this;
    }

    internal TodosTestBuilder GetTodo(string description)
    {
        With(async httpClient =>
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri: $"/Todos/{_todo?.Id}");
            var httpResponse = await httpClient.SendAsync(httpRequest);
            var responsePayload = await httpResponse.Content.ReadFromJsonAsync<GetTodo.Response>();
            _todo = new(_todo?.Id, responsePayload!.Title, responsePayload.Tags, responsePayload.Done);

            return new Actual(description,
                Request: Request.Create(httpRequest),
                Response: Response.Create(httpResponse, responsePayload));
        });
        return this;
    }

    internal TodosTestBuilder DoneTodo(string description)
    {
        With(async httpClient =>
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{_todo?.Id}");
            var requestPayload = new UpdateTodo.Request(Title: _todo!.Title!, Tags: _todo!.Tags!, Done: true);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);
            _todo = _todo with {Done = true};

            return new Actual(description,
                Request: Request.Create(httpRequest, requestPayload),
                Response: Response.Create(httpResponse));
        });
        return this;
    }

    internal TodosTestBuilder ChangeTitle(string description, string? newTitle)
    {
        With(async httpClient =>
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{_todo?.Id}");
            var requestPayload = new UpdateTodo.Request(Title: newTitle!, Tags: _todo!.Tags!, _todo.Done!.Value);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);
            _todo = _todo with {Title = newTitle};

            return new Actual(description,
                Request: Request.Create(httpRequest, requestPayload),
                Response: Response.Create(httpResponse));
        });
        return this;
    }

    internal TodosTestBuilder ChangeTags(string description, string?[]? newTags)
    {
        With(async httpClient =>
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{_todo?.Id}");
            var requestPayload = new UpdateTodo.Request(Title: _todo!.Title!, Tags: newTags!, _todo.Done!.Value);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);
            _todo = _todo with {Tags = newTags};

            return new Actual(description,
                Request: Request.Create(httpRequest, requestPayload),
                Response: Response.Create(httpResponse));
        });
        return this;
    }

    private sealed record Todo(string? Id, string? Title = null, string?[]? Tags = null, bool? Done = null);
}