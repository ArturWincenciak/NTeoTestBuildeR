using System.Net;
using System.Net.Http.Json;
using NTeoTestBuildeR.Modules.Todos.Api;
using TeoTests.Modules.TodosModule.Builder;

namespace TeoTests.Modules.TodosModule;

public class TodosHappyPathTests
{
    [Fact]
    public async Task CreateTodo()
    {
        // act
        var title = "Prove Riemann's hypothesis";
        var tag = "match";

        // act
        var actual = await new TodosTestBuilder()
            .CreateTodo(title, tags: [tag])
            .GetTodo()
            .Build();

        // assert
        var creatHttpResponse = actual.First();
        Assert.Equal(HttpStatusCode.Created, creatHttpResponse.StatusCode);
        var createdPayloadResponse = await creatHttpResponse.Content.ReadFromJsonAsync<CreateTodo.Response>();
        Assert.True(createdPayloadResponse!.Id != Guid.Empty);

        var getActual = actual.Last();
        Assert.Equal(HttpStatusCode.OK, getActual.StatusCode);
        var getPayloadResponse = await getActual.Content.ReadFromJsonAsync<GetTodo.Response>();
        Assert.Equal(title, getPayloadResponse!.Title);
        Assert.Equivalent(tag, actual: getPayloadResponse.Tags.Single());
    }
}