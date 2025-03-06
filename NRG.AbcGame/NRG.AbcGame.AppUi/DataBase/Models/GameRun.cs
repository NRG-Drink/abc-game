namespace NRG.AbcGame.AppUi.DataBase.Models;

public record GameRun
{
    public required string Topic { get; init; }
    public required DateTime StartUTC { get; init; }
    public required TimeSpan Time { get; init; }
    public Dictionary<char, List<GameInput>> Values { get; init; } = [];
}

public record GameInput
{
    public required string Value { get; init; }
    public required DateTime TimeUTC { get; init; }
}
