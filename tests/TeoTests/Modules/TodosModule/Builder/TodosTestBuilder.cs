using System.Net.Http.Json;
using NTeoTestBuildeR.Modules.Todos.Api;
using TeoTests.Core;
using TeoTests.Core.Verify;

namespace TeoTests.Modules.TodosModule.Builder;

internal sealed class TodosTestBuilder : TestBuilder
{
    private Uri? _todoLocation;

    internal TodosTestBuilder CreateTodo(string description, string? title, string?[]? tags)
    {
        With(async httpClient =>
        {
            var requestPayload = new CreateTodo.Request(Title: title!, Tags: tags!);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri: "/todos");
            httpRequest.Content = JsonContent.Create(requestPayload);
            var httpResponse = await httpClient.SendAsync(httpRequest);
            _todoLocation = httpResponse.Headers.Location;
            var responsePayload = await httpResponse.Content.ReadFromJsonAsync<CreateTodo.Response>();

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
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, _todoLocation);
            var httpResponse = await httpClient.SendAsync(httpRequest);
            var responsePayload = await httpResponse.Content.ReadFromJsonAsync<GetTodo.Response>();

            return new Actual(description,
                Request: Request.Create(httpRequest),
                Response: Response.Create(httpResponse, responsePayload));
        });
        return this;
    }
}