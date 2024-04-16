using JetBrains.Annotations;

namespace NTeoTestBuildeR.Modules.Stats.Api;

[PublicAPI]
public sealed record GetStats(GetStats.Query Dto)
{
    [PublicAPI]
    public sealed record Query(string[] Tags);

    [PublicAPI]
    public sealed record Response(Item[] Stats);

    [PublicAPI]
    public sealed record Item(string Tag, int Count);
}