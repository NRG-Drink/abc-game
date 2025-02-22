using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NRG.AbcGame.UiComponents;

public class MenuOptionExit(string name) 
    : MenuOptionBase(name, string.Empty, e => true, true, false);
public class MenuOption(string name, string value, Predicate<string> validationFunc) 
    : MenuOptionBase(name, value, validationFunc, false, true);
public class MenuOption<T>(
    string name,
    string value,
    Predicate<string> validationFunc,
    Func<string, T> toValueFunc
    ) 
    : MenuOption(name, value, validationFunc)
{
    public T ValueTyped => toValueFunc(Value);
}
public class MenuOptionExecute(string name, Func<Task> asyncFunc) 
    : MenuOptionBase(name, string.Empty, e => true, false, false)
{
    public override Task SetUserValueAsync(int lineX, int lineY)
        => asyncFunc();
}

public abstract class MenuOptionBase(
    string name,
    string value,
    Predicate<string> validFunc,
    bool isMenuExit,
    bool isValueShown
    )
{
    private bool _isCancelled = false;

    public string Name => name;
    public string Value { get; private set; } = value;
    public Predicate<string> IsValidFunc => validFunc;
    public bool IsMenuExit => isMenuExit;
    public bool IsValueShown => isValueShown;

    public virtual async Task SetUserValueAsync(int lineX, int lineY)
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
