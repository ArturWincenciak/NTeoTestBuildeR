using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.Extensions;
using NTeoTestBuildeR.Modules.Todos.Api;
using TeoTests.Core;
using TeoTests.Core.Verify;

namespace TeoTests.Modules.TodosModule.Builder;

internal sealed class TodosTestBuilder : TestBuilder<TodosTestBuilder>
{
    private readonly TodosTestState _state = new();

    internal TodosTestBuilder CreateTodo(string description, string? title, string?[]? tags)
    {
        With(async () =>
        {
            var requestPayload = new CreateTodo.Request(Title: title!, Tags: tags!);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri: "/todos");
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await SendAsync(httpRequest);

            return await httpResponse.DeserializeWith<CreateTodo.Response>(success: resultPayload =>
                    _state.Upsert(id: resultPayload!.Id.ToString(), title: title!, tags: tags!, done: false),
                description, httpRequest, requestPayload);
        });
        return this;
    }

    internal TodosTestBuilder GetTodo(string description, string whichTitle)
    {
        With(async () =>
        {
            var testCase = _state.SelectByTitle(whichTitle);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri: $"/Todos/{testCase.Id}");
            var httpResponse = await SendAsync(httpRequest);

            return await httpResponse.DeserializeWith<GetTodo.Response>(success: resultPayload =>
                    _state.Upsert(id: testCase.Id!, resultPayload!.Title, resultPayload.Tags, resultPayload.Done),
                description, httpRequest);
        });
        return this;
    }

    internal TodosTestBuilder GetTodo(string description, Guid? id)
    {
        With(async () =>
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri: $"/todos/{id}");
            var httpResponse = await SendAsync(httpRequest);
            return await httpResponse.Deserialize(description, httpRequest);
        });
        return this;
    }

    internal TodosTestBuilder DoneTodo(string description, string whichTitle)
    {
        With(async () =>
        {
            var testCase = _state.SelectByTitle(whichTitle);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{testCase.Id}");
            var requestPayload = new UpdateTodo.Request(Title: testCase.Title!, Tags: testCase.Tags!, Done: true);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await SendAsync(httpRequest);

            return await httpResponse.DeserializeWith(success: () =>
                    _state.Upsert(id: testCase.Id!, title: testCase.Title!, tags: testCase.Tags!, done: true),
                description, httpRequest, requestPayload);
        });
        return this;
    }

    internal TodosTestBuilder ChangeTitle(string description, string oldTitle, string? newTitle)
    {
        With(async () =>
        {
            var testCase = _state.SelectByTitle(oldTitle);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{testCase.Id}");
            var requestPayload = new UpdateTodo.Request(Title: newTitle!, Tags: testCase.Tags!, testCase.Done!.Value);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await SendAsync(httpRequest);

            return await httpResponse.DeserializeWith(success: () =>
                    _state.Upsert(id: testCase.Id!, title: newTitle!, tags: testCase.Tags!, testCase.Done!.Value),
                description, httpRequest, requestPayload);
        });
        return this;
    }

    internal TodosTestBuilder ChangeTags(string description, string whichTitle, string?[]? newTags)
    {
        With(async () =>
        {
            var testCase = _state.SelectByTitle(whichTitle);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{testCase.Id}");
            var requestPayload = new UpdateTodo.Request(Title: testCase.Title!, Tags: newTags!, testCase.Done!.Value);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await SendAsync(httpRequest);

            return await httpResponse.DeserializeWith(success: () =>
                    _state.Upsert(id: testCase.Id!, title: testCase.Title!, tags: newTags!, testCase.Done!.Value),
                description, httpRequest, requestPayload);
        });
        return this;
    }

    internal TodosTestBuilder GetTodos(string description, string?[]? tags)
    {
        With(async () =>
        {
            var query = new QueryBuilder {{"tags", tags!}};
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri: $"/todos{query.ToQueryString()}");
            var httpResponse = await SendAsync(httpRequest);

            return await httpResponse.DeserializeWith<GetTodos.Response>(
                success: resultPayload =>
                {
                    var sortedItems = resultPayload!.Todos
                        .OrderBy(todo => todo.Title)
                        .ToArray();

                    return Actual.Create(description, httpRequest, httpResponse, requestPayload: null,
                        responsePayload: new GetTodos.Response(sortedItems));
                }, description, httpRequest);
        });
        return this;
    }
}