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
        await Verify(actual);

        // *** or like that ***
        // var creatHttpResponse = actual.First();
        // var createdPayloadResponse = await creatHttpResponse.Content.ReadFromJsonAsync<CreateTodo.Response>();
        // var getActual = actual.Last();
        // var getPayloadResponse = await getActual.Content.ReadFromJsonAsync<GetTodo.Response>();
        // await Verify(new {createdPayloadResponse, getPayloadResponse});
    }
}