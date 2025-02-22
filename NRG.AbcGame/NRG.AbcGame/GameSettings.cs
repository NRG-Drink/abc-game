using NRG.AbcGame.UiComponents;
using System.Diagnostics;

namespace NRG.AbcGame;

public class GameSettings(string baseFolder)
{
    private static readonly MenuOptionExit _start = new("Start");
    private static  readonly MenuOptionExit _exit = new("Exit");
    private static readonly MenuOption _topic = new("Topic", "Animals", e => true);
    private static readonly MenuOption _time = new("Time", TimeSpan.FromSeconds(180).ToString("c"), e => DateTime.TryParse(e, out var _));
    private readonly MenuOptionExecute _openFolder = new("Show Runs", () => OpenFolder(baseFolder));

    public TimeSpan Time { get; private set; }
    public string Topic { get; private set; } = string.Empty;

    public async Task<bool> RunMenu(int line)
    {
        var menuOptions = GetGameMenu();
        var menu = new Menu(menuOptions, line, ConsoleColor.Yellow);
        var (selected, isCancelled) = await menu.ChooseOptionAsync();

        if (selected == _exit)
        {
            return true;
        }

        Console.SetCursorPosition(0, line + menuOptions.Length);

        Time = DateTime.Parse(_time.Value).TimeOfDay;
        Topic = _topic.Value;

        return isCancelled;
    }

    private MenuOptionBase[] GetGameMenu()
        => [_start, _topic, _time, _openFolder, _exit];

    private void CancelMenu(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
    }

    private static Task OpenFolder(string folder)
    {
        Process.Start("explorer.exe", folder);
        return Task.CompletedTask;
    }
}
