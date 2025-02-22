using NRG.AbcGame.Models;
using NRG.AbcGame.UiComponents;
using System.Diagnostics;

namespace NRG.AbcGame;

public class GameMenu(string baseFolder, GameSettings defaults, Func<GameSettings, Task> saveSettingsFunc)
{
    private static readonly MenuOptionExit _start = new("Start");
    private static readonly MenuOptionExit _exit = new("Exit");
    private readonly MenuOption _topic = new("Topic", defaults.Topic, e => true);
    private readonly MenuOption<TimeSpan> _time = new("Time", defaults.Time, e => DateTime.TryParse(e, out var _), e => DateTime.Parse(e).TimeOfDay);
    private readonly MenuOption<int> _countdown = new("Start Countdown", defaults.StartCountdown.ToString(), e => int.TryParse(e, out var _), int.Parse);
    private readonly MenuOption<int> _extraTime = new("Extra Time (s)", defaults.ExtraTime.ToString(), e => int.TryParse(e, out var _), int.Parse);
    private readonly MenuOptionExecute _openFolder = new("Show Runs", () => OpenFolder(baseFolder));
    private MenuOptionExecute _saveSettings = null!;

    public string Topic => _topic.Value;
    public TimeSpan Time => _time.ValueTyped;
    public int StartCountdown => _countdown.ValueTyped;
    public int ExtraTime => _extraTime.ValueTyped;

    public async Task<bool> RunMenu(int line)
    {
        _saveSettings = new("Save Settings", SaveSettings);
        var menuOptions = GetGameMenu();
        var menu = new Menu(menuOptions, line, ConsoleColor.Yellow);
        var (selected, isCancelled) = await menu.ChooseOptionAsync();

        if (selected == _exit)
        {
            return true;
        }

        Console.SetCursorPosition(0, line + menuOptions.Length);

        return isCancelled;
    }

    private MenuOptionBase[] GetGameMenu()
        => [_start, _topic, _time, _countdown, _extraTime, _openFolder, _saveSettings, _exit];

    private void CancelMenu(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
    }

    private static Task OpenFolder(string folder)
    {
        Process.Start("explorer.exe", folder);
        return Task.CompletedTask;
    }

    private Task SaveSettings() 
        => saveSettingsFunc(new(Topic, Time.ToString("c"), StartCountdown, ExtraTime));
}
