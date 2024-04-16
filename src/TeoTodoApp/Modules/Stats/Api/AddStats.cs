using JetBrains.Annotations;

namespace NTeoTestBuildeR.Modules.Stats.Api;

[PublicAPI]
public sealed record AddStats(AddStats.Request Dto)
{
    [PublicAPI]
    public sealed record Request(string Tag);
}