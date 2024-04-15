using System.Net.Http.Json;
using NTeoTestBuildeR.Modules.Todos.Api;
using TeoTests.Core;
using TeoTests.Core.Verify;

namespace TeoTests.Modules.TodosModule.Builder;

internal sealed class TodosTestBuilder : TestBuilder
{
    private readonly TodosTestState _state = new();

    internal TodosTestBuilder CreateTodo(string description, string? title, string?[]? tags)
    {
        With(async httpClient =>
        {
            var requestPayload = new CreateTodo.Request(Title: title!, Tags: tags!);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri: "/todos");
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responsePayload = await httpResponse.Content.ReadFromJsonAsync<CreateTodo.Response>();
                _state.Upsert(id: responsePayload!.Id.ToString(), title: title!, tags: tags!, done: false);

                return new Actual(description,
                    Request: Request.Create(httpRequest, requestPayload),
                    Response: Response.Create(httpResponse, responsePayload));
            }

            var error = await httpResponse.Deserialize();
            return new Actual(description,
                Request: Request.Create(httpRequest, requestPayload),
                Response: Response.Create(httpResponse, error));
        });
        return this;
    }

    internal TodosTestBuilder GetTodo(string description, string whichTitle)
    {
        With(async httpClient =>
        {
            var testCase = _state.SelectByTitle(whichTitle);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri: $"/Todos/{testCase.Id}");
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responsePayload = await httpResponse.Content.ReadFromJsonAsync<GetTodo.Response>();
                _state.Upsert(id: testCase.Id!, responsePayload!.Title, responsePayload.Tags, responsePayload.Done);

                return new Actual(description,
                    Request: Request.Create(httpRequest),
                    Response: Response.Create(httpResponse, responsePayload));
            }

            var error = await httpResponse.Deserialize();
            return new Actual(description,
                Request: Request.Create(httpRequest),
                Response: Response.Create(httpResponse, error));
        });
        return this;
    }

    internal TodosTestBuilder DoneTodo(string description, string whichTitle)
    {
        With(async httpClient =>
        {
            var testCase = _state.SelectByTitle(whichTitle);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{testCase.Id}");
            var requestPayload = new UpdateTodo.Request(Title: testCase.Title!, Tags: testCase.Tags!, Done: true);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                _state.Upsert(id: testCase.Id!, title: testCase.Title!, tags: testCase.Tags!, done: true);

                return new Actual(description,
                    Request: Request.Create(httpRequest, requestPayload),
                    Response: Response.Create(httpResponse));
            }

            var error = await httpResponse.Deserialize();
            return new Actual(description,
                Request: Request.Create(httpRequest, requestPayload),
                Response: Response.Create(httpResponse, error));
        });
        return this;
    }

    internal TodosTestBuilder ChangeTitle(string description, string oldTitle, string? newTitle)
    {
        With(async httpClient =>
        {
            var testCase = _state.SelectByTitle(oldTitle);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{testCase.Id}");
            var requestPayload = new UpdateTodo.Request(Title: newTitle!, Tags: testCase.Tags!, testCase.Done!.Value);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                _state.Upsert(id: testCase.Id!, title: newTitle!, tags: testCase.Tags!, testCase.Done!.Value);

                return new Actual(description,
                    Request: Request.Create(httpRequest, requestPayload),
                    Response: Response.Create(httpResponse));
            }

            var error = await httpResponse.Deserialize();
            return new Actual(description,
                Request: Request.Create(httpRequest, requestPayload),
                Response: Response.Create(httpResponse, error));
        });
        return this;
    }

    internal TodosTestBuilder ChangeTags(string description, string whichTitle, string?[]? newTags)
    {
        With(async httpClient =>
        {
            var testCase = _state.SelectByTitle(whichTitle);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri: $"/Todos/{testCase.Id}");
            var requestPayload = new UpdateTodo.Request(Title: testCase.Title!, Tags: newTags!, testCase.Done!.Value);
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                _state.Upsert(id: testCase.Id!, title: testCase.Title!, tags: newTags!, testCase.Done!.Value);

                return new Actual(description,
                    Request: Request.Create(httpRequest, requestPayload),
                    Response: Response.Create(httpResponse));
            }

            var error = await httpResponse.Deserialize();
            return new Actual(description,
                Request: Request.Create(httpRequest, requestPayload),
                Response: Response.Create(httpResponse, error));
        });
        return this;
    }
}