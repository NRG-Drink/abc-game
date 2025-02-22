namespace NRG.AbcGame.Models;

public record GameRun
{
    public bool IsCancelled => MaxTime != GameTime;
    public required TimeSpan MaxTime { get; init; }
    public required TimeSpan GameTime { get; init; }
    public required string Topic { get; init; }
    public required IDictionary<char, List<string>> Values { get; init; }

}
