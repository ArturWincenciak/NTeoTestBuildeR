using Microsoft.AspNetCore.Mvc;
using NTeoTestBuildeR.Modules.Todos.Core.Services;
using ErrorHandling_ProblemDetails = NTeoTestBuildeR.Infra.ErrorHandling.ProblemDetails;

namespace NTeoTestBuildeR.Modules.Todos.Api;

[ApiController]
[Consumes("application/json")]
[Route(TODOS)]
[ProducesResponseType(type: typeof(ErrorHandling_ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class TodosController(TodosService service) : ControllerBase
{
    private const string TODOS = "Todos";

    [HttpPost]
    [ProducesResponseType(type: typeof(CreateTodo.Response), StatusCodes.Status201Created)]
    [ProducesResponseType(type: typeof(ErrorHandling_ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(CreateTodo.Request body)
    {
        var response = await service.Create(new(body));
        return Created(uri: $"/{TODOS}/{response.Id}", response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(type: typeof(ErrorHandling_ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(type: typeof(ErrorHandling_ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(Guid id, UpdateTodo.Request body)
    {
        await service.Update(new(id, body));
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(type: typeof(GetTodo.Response), StatusCodes.Status200OK)]
    [ProducesResponseType(type: typeof(ErrorHandling_ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(type: typeof(ErrorHandling_ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(Guid id) =>
        Ok(await service.GetTodo(new(new(id))));

    [HttpGet]
    [ProducesResponseType(type: typeof(GetTodos.Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetTodos.Query query) =>
        Ok(await service.GetTodos(query));
}