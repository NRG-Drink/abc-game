namespace NRG.AbcGame.Models;

public record GameSettings(
    string Topic = "Animals",
    string GameTime = "00:03:00",
    int StartCountdown = 3,
    int ExtraTimeAdd = 10,
    bool IsExtraTimeEnabled = true
    );
