namespace NRG.AbcGame.UiComponents;

public class Menu(
    MenuOption[] menuOptions,
    int menuTopLine,
    ConsoleColor accentColor = ConsoleColor.DarkGreen
    )
{
    private readonly int _maxBound = menuOptions.Length - 1;
    private int _menuSelect = 0;
    private bool _menuCancel = false;

    public MenuOption[] Options => menuOptions;

    public async Task<(MenuOption Option, bool IsCancelled)> ChooseOptionAsync()
    {
        Console.CursorVisible = false;
        Console.CancelKeyPress += CancelMenu;

        PrintMenu();

        while (!_menuCancel)
        {
            var input = await ReadKeyOrDefaultAndDelayAsync();

            if (input.Key is ConsoleKey.UpArrow)
            {
                _menuSelect = Math.Clamp(--_menuSelect, 0, _maxBound);
            }
            else if (input.Key is ConsoleKey.DownArrow)
            {
                _menuSelect = Math.Clamp(++_menuSelect, 0, _maxBound);
            }
            else if (input.Key is ConsoleKey.Enter && menuOptions[_menuSelect].IsMenuExit)
            {
                break;
            }
            else if (input.Key is ConsoleKey.Enter)
            {
                var selected = menuOptions[_menuSelect];
                var x = selected.Name.Length + 5;
                Console.CancelKeyPress -= CancelMenu;
                await selected.SetUserValueAsync(x, menuTopLine + _menuSelect);
                Console.CancelKeyPress += CancelMenu;
                Console.CursorVisible = false;
            }

            PrintMenu();
        }

        Console.CancelKeyPress -= CancelMenu;
        Console.CursorVisible = true;

        return (menuOptions[_menuSelect], _menuCancel);
    }

    private void CancelMenu(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        _menuCancel = true;
    }

    private async Task<ConsoleKeyInfo> ReadKeyOrDefaultAndDelayAsync()
    {
        var input = new ConsoleKeyInfo();
        if (Console.KeyAvailable)
        {
            input = Console.ReadKey();
        }
        else
        {
            await Task.Delay(50);
        }

        return input;
    }

    private void PrintMenu()
    {
        Console.SetCursorPosition(0, menuTopLine);
        foreach (var e in menuOptions)
        {
            var str = e.IsMenuExit
                ? $"  {e.Name}"
                : $"  {e.Name} - {e.Value}";
            Console.WriteLine(str);
        }

        var color = Console.ForegroundColor;
        Console.SetCursorPosition(0, menuTopLine + _menuSelect);
        Console.ForegroundColor = accentColor;
        Console.Write(">");
        Console.ForegroundColor = color;
        Console.SetCursorPosition(0, menuTopLine + menuOptions.Length);
    }
}
