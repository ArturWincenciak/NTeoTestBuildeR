using TeoTests.Modules.StatsModule.Builder;

namespace TeoTests.Modules.StatsModule;

public class StatsAppTests
{
    [Fact]
    public async Task CollectAndGetStatistics()
    {
        // act
        var actual = await new StatsTestBuilder()
            .AddStats(description: "Collect first work tag", tag: "work")
            .AddStats(description: "Collect second work tag", tag: "work")
            .AddStats(description: "Collect one taxes tag", tag: "taxes")
            .AddStats(description: "Collect one holiday tag", tag: "holiday")
            .AddStats(description: "Collect first car tag", tag: "car")
            .AddStats(description: "Collect second car tag", tag: "car")
            .GetStats(description: "Retrieve work stats", tags: ["work"])
            .GetStats(description: "Retrieve taxes stats", tags: ["taxes"])
            .GetStats(description: "Retrieve car stats", tags: ["car"])
            .GetStats(description: "Retrieve holiday stats", tags: ["holiday"])
            .GetStats(description: "Retrieve work and car stats", tags: ["work", "car"])
            .GetStats(description: "Retrieve work and taxes stats", tags: ["work", "taxes"])
            .GetStats(description: "Retrieve work, car, and taxes stats", tags: ["work", "car", "taxes"])
            .GetStats(description: "Retrieve all stats", tags: ["work", "car", "taxes", "holiday"])
            .GetStats(description: "Retrieve all stats", tags: ["work", "car", "taxes", "holiday"])
            .DeleteStats(description: "Remove first work tag", tag: "work")
            .GetStats(description: "Retrieve work tag (expected value is one)", tags: ["work"])
            .DeleteStats(description: "Remove second work tag", tag: "work")
            .GetStats(description: "Retrieve work tag (expected value is zero)", tags: ["work"])
            .Build();

        // assert
        await Verify(actual);
    }
}