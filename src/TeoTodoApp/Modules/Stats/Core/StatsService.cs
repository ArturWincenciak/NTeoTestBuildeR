using NTeoTestBuildeR.Modules.Stats.Api;

namespace NTeoTestBuildeR.Modules.Stats.Core;

public static class StatsService
{
    private readonly static Dictionary<string, int> Stats = new();

    public static void AddStats(AddStats cmd) =>
        Stats[cmd.Dto.Tag] = Stats.TryGetValue(cmd.Dto.Tag, value: out var value)
            ? value + 1
            : 1;

    public static GetStats.Response GetStats(GetStats query) =>
        new(Stats
            .Where(stat => query.Dto.Tags.Contains(stat.Key))
            .Select(stat => new GetStats.Item(stat.Key, stat.Value))
            .ToArray());

    public static void DeleteStats(DeleteStats cmd)
    {
        if (Stats.TryGetValue(cmd.Tag, value: out var value))
            if (value > 0)
                Stats[cmd.Tag] = value - 1;
    }
}