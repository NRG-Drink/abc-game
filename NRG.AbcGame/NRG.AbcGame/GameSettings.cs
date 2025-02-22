using NRG.AbcGame.UiComponents;

namespace NRG.AbcGame;

public class GameSettings
{
    private static readonly MenuOption _start = new("Start", true);
    private static  readonly MenuOption _exit = new("Exit", true);
    private static readonly MenuOption _time = new("Time", TimeSpan.FromSeconds(10).ToString("c"), e => DateTime.TryParse(e, out var _));
    private static readonly MenuOption _topic = new("Topic", "Animals");
    private readonly MenuOption[] _menuOptions =
    [
        _start,
        _topic,
        _time,
        _exit,
    ];

    public TimeSpan Time { get; private set; }
    public string Topic { get; private set; } = string.Empty;

    public async Task<bool> RunMenu(int line)
    {
        var menu = new Menu(_menuOptions, line, ConsoleColor.Yellow);
        var (selected, isCancelled) = await menu.ChooseOptionAsync();

        if (selected == _exit)
        {
            return true;
        }

        Console.SetCursorPosition(0, line + _menuOptions.Length);

        Time = DateTime.Parse(_time.Value).TimeOfDay;
        Topic = _topic.Value;

        return isCancelled;
    }

    private void CancelMenu(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
    }
}
