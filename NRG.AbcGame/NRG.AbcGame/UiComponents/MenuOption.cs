using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NRG.AbcGame.UiComponents;

public class MenuOption
{
    private bool _isCancelled = false;

    [SetsRequiredMembers]
    public MenuOption(string name, bool isExit)
    {
        Name = name;
        Value = string.Empty;
        IsMenuExit = isExit;
    }

    [SetsRequiredMembers]
    public MenuOption(string name, string value)
    {
        Name = name;
        Value = value;
        IsMenuExit = false;
    }

    [SetsRequiredMembers]
    public MenuOption(string name, string value, Predicate<string> validateFunc)
    {
        Name = name;
        Value = value;
        IsMenuExit = false;
        IsValidFunc = validateFunc;
    }

    public required string Name { get; init; }
    public required string Value { get; set; }
    public Predicate<string> IsValidFunc { get; init; } = e => true;
    public bool IsMenuExit { get; init; } = false;

    public async Task SetUserValueAsync(int lineX, int lineY)
    {
        _isCancelled = false;
        var str = new StringBuilder(Value);
        Console.CursorVisible = true;
        Console.CancelKeyPress += CancelMenu;
        Console.SetCursorPosition(lineX, lineY);
        Console.Write(Value);

        while (!_isCancelled)
        {
            var key = await ReadKeyOrDefaultAndDelayAsync();

            if (key.Key is ConsoleKey.None)
            {
                continue;
            }

            if (key.Key is ConsoleKey.Backspace)
            {
                if (str.Length < 1)
                {
                    Console.SetCursorPosition(lineX, lineY);
                    continue;
                }

                str.Remove(str.Length - 1, 1);
                Console.SetCursorPosition(lineX + str.Length, lineY);
                Console.Write(' ');
                Console.SetCursorPosition(lineX + str.Length, lineY);
            }
            else if (key.Key is ConsoleKey.Enter)
            {
                break;
            }
            else
            {
                str.Append(key.KeyChar);
            }
        }

        Console.CancelKeyPress -= CancelMenu;

        if (_isCancelled)
        {
            return;
        }

        var val = str.ToString();
        if (IsValidFunc(val))
        {
            Value = str.ToString();
        }
    }

    private void CancelMenu(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        _isCancelled = true;
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
}
