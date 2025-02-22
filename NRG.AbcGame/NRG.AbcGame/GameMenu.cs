using NRG.AbcGame.Models;
using NRG.AbcGame.UiComponents;
using System.Diagnostics;

namespace NRG.AbcGame;

public class GameMenu(
    string baseFolder,
    GameSettings defaults,
    Func<GameSettings, Task> saveSettingsFunc
    )
{
    private static readonly MenuOptionExit _start = new("Start");
    private static readonly MenuOptionExit _exit = new("Exit");
    private readonly MenuOption _topic = new("Topic", defaults.Topic, e => true);
    private readonly MenuOption<TimeSpan> _time = new("Time", defaults.GameTime, e => DateTime.TryParse(e, out var _), e => DateTime.Parse(e).TimeOfDay);
    private readonly MenuOption<int> _countdown = new("Start Countdown", defaults.StartCountdown.ToString(), e => int.TryParse(e, out var _), int.Parse);
    private readonly MenuOption<int> _extraTime = new("Extra Time (s)", defaults.ExtraTimeAdd.ToString(), e => int.TryParse(e, out var _), int.Parse);
    private readonly MenuOption<bool> _isExtraTime = new("Enable Extra Time", defaults.IsExtraTimeEnabled.ToString().ToLower(), e => bool.TryParse(e, out var _), bool.Parse);
    private readonly MenuOptionExecute _openFolder = new("Show Runs", () => OpenFolder(baseFolder));
    private MenuOptionExecute _saveSettings = null!;

    public async Task<(bool, GameSettings)> RunMenu(int line)
    {
        _saveSettings = new("Save Settings", SaveSettings);
        var menuOptions = GetGameMenu();
        var menu = new Menu(menuOptions, line, ConsoleColor.Yellow);
        var (selected, isCancelled) = await menu.ChooseOptionAsync();

        if (selected == _exit)
        {
            return (true, new());
        }

        Console.SetCursorPosition(0, line + menuOptions.Length);

        var settings = ExtractSettings();
        return (isCancelled, settings);
    }

    private MenuOptionBase[] GetGameMenu()
        =>
        [
            _start,
            _topic,
            _time,
            _countdown,
            _extraTime,
            _isExtraTime,
            _openFolder,
            _saveSettings,
            _exit
        ];

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
        => saveSettingsFunc(ExtractSettings());

    private GameSettings ExtractSettings()
        => new(
            _topic.Value,
            _time.Value,
            _countdown.ValueTyped,
            _extraTime.ValueTyped,
            _isExtraTime.ValueTyped
        );
}
