using Microsoft.AspNetCore.Mvc;
using NTeoTestBuildeR.Modules.Stats.Core;

namespace NTeoTestBuildeR.Modules.Stats.Api;

[ApiController]
[Consumes("application/json")]
[Route(STATS)]
public sealed class StatsController : ControllerBase
{
    private const string STATS = "Stats";

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Post([FromBody] AddStats.Request body)
    {
        StatsService.AddStats(new(body));
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(type: typeof(GetStats.Query), StatusCodes.Status200OK)]
    public IActionResult Get([FromQuery] GetStats.Query query) =>
        Ok(StatsService.GetStats(new(query)));

    [HttpDelete("{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Get([FromRoute] string tag)
    {
        StatsService.DeleteStats(new(tag));
        return NoContent();
    }
}