namespace NRG.AbcGame.Models;

public record GameSettings(
    string Topic = "Animals",
    string Time = "00:03:00",
    int StartCountdown = 3,
    int ExtraTime = 10
    ); 
