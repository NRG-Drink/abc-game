namespace NRG.AbcGame.Models;

public record GameRun
{
    public required TimeSpan Time { get; init; }
    public required string Topic { get; init; }
    public required IDictionary<char, List<string>> Values { get; init; }
}
