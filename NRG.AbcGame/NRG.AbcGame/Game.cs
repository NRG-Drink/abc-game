using NRG.AbcGame.Models;
using System.Diagnostics;
using System.Text;

namespace NRG.AbcGame;

public class Game(GameSettings settings)
{
    private readonly Dictionary<char, List<string>> _wordList = InitValues();
    private readonly StringBuilder _inputWord = new();
    private Stopwatch _sw = new();
    private TimeSpan _playTime = DateTime.Parse(settings.GameTime).TimeOfDay;
    private int _inputLine;
    private int _timeLine;
    private int _resultLine;
    private bool _isCancelled;
    private bool _isTimeExceeded;

    public async Task<GameRun> RunGame(int line)
    {
        _inputLine = line;
        _resultLine = line + 3;
        _timeLine = _resultLine - 1;
        Console.CancelKeyPress += CancelMenu;
        long lastTimePrint = 0;
        _isCancelled = false;

        await PrintCountdownAsync(settings.StartCountdown);


        PrintWorldList();
        PrintTime();
        Console.SetCursorPosition(_inputWord.Length, _inputLine);
        _sw = Stopwatch.StartNew();

        while (!_isCancelled && !_isTimeExceeded)
        {
            await HandleKeyStrokeAsync();

            var diff = _sw.ElapsedMilliseconds - lastTimePrint;
            if (diff > 1_000)
            {
                _isTimeExceeded = _sw.Elapsed > _playTime;
                lastTimePrint = _sw.ElapsedMilliseconds;
                PrintTime();
                Console.SetCursorPosition(_inputWord.Length, _inputLine);
            }

            if (_isTimeExceeded && settings.IsExtraTimeEnabled)
            {
                _sw.Stop();
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(0, _inputLine + 1);
                Console.Write($"Buy {settings.ExtraTimeAdd} seconds overtime? (y|n) ");
                Console.ForegroundColor = color;
                var exceedInput = Console.ReadLine();
                if (exceedInput?.StartsWith("y", StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    _isTimeExceeded = false;
                    lastTimePrint = 0;
                    _playTime = _playTime.Add(TimeSpan.FromSeconds(settings.ExtraTimeAdd));
                    Console.SetCursorPosition(0, _inputLine + 1);
                    Console.Write(Enumerable.Repeat(' ', 35).ToArray());
                    Console.SetCursorPosition(_inputWord.Length, _inputLine);
                    _sw.Start();
                }
            }
        }

        _sw.Stop();
        Console.CancelKeyPress -= CancelMenu;
        Console.SetCursorPosition(0, _resultLine + _wordList.Count);

        return new()
        {
            MaxTime = _playTime,
            GameTime = _isTimeExceeded ? _playTime : _sw.Elapsed,
            Topic = settings.Topic,
            Values = _wordList
        };
    }

    public async Task HandleKeyStrokeAsync()
    {
        var key = await ReadKeyOrDefaultAndDelayAsync();

        if (key.Key is ConsoleKey.None)
        {
            return;
        }

        if (key.Key is ConsoleKey.Backspace)
        {
            var (x, y) = Console.GetCursorPosition();
            if (_inputWord.Length < 1)
            {
                Console.SetCursorPosition(x + 1, y);
                return;
            }

            _inputWord.Remove(_inputWord.Length - 1, 1);
            Console.SetCursorPosition(x, y);
            Console.Write(' ');
            Console.SetCursorPosition(x, y);
        }
        else if (key.Key is ConsoleKey.Enter && _inputWord.Length > 0)
        {
            var first = _inputWord[0].ToString().ToUpper()[0];
            if (_wordList.TryGetValue(first, out var values))
            {
                values.Add(_inputWord.ToString());

                var blank = Enumerable.Repeat(' ', _inputWord.Length).ToArray();
                Console.Write(blank);

                _inputWord.Clear();
                PrintWorldList();
            }
        }
        else
        {
            _inputWord.Append(key.KeyChar);
        }
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

    private async Task PrintCountdownAsync(int seconds)
    {
        var template = "Prepare, the game will start in {0} seconds. ";
        var blank = Enumerable.Repeat(' ', template.Length).ToArray();
        Console.CursorVisible = false;
        for (var i = seconds; i > 0 && !_isCancelled; i--)
        {
            Console.SetCursorPosition(0, _inputLine);
            Console.Write(template, i);
            await Task.Delay(980);
        }

        Console.SetCursorPosition(0, _inputLine);
        Console.Write(blank);

        if (_isCancelled)
        {
            Console.SetCursorPosition(0, _inputLine);
            Console.WriteLine("Game was cancelled.");
            Console.CursorVisible = true;
            return;
        }

        Console.SetCursorPosition(0, _inputLine);
        Console.Write($"Game Start!");
        await Task.Delay(700);
        Console.SetCursorPosition(0, _inputLine);
        Console.Write(blank);
        Console.CursorVisible = true;
    }

    private void PrintTime()
    {
        var color = Console.ForegroundColor;
        //var time = _isTimeExceeded ? _playTime : _sw.Elapsed;
        var time = _playTime - _sw.Elapsed is var t && t > TimeSpan.Zero ? t : TimeSpan.Zero;

        if (time <= TimeSpan.FromSeconds(5))
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }

        Console.CursorVisible = false;
        Console.SetCursorPosition(0, _timeLine);
        Console.Write($"time: {time:hh\\:mm\\:ss\\.fff}");
        Console.CursorVisible = true;
        Console.ForegroundColor = color;
    }

    public void PrintWorldList()
    {
        Console.SetCursorPosition(0, _resultLine);
        var text = _wordList
            .Aggregate(
                new StringBuilder(),
                (acc, e) => acc.AppendLine($"{e.Key} ({e.Value.Count}) - {string.Join(", ", e.Value)}")
            )
            .ToString();

        Console.Write(text);
    }

    private void CancelMenu(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        _isCancelled = true;
    }

    private static Dictionary<char, List<string>> InitValues()
        => Enumerable
            .Range('A', 26)
            .Order()
            .ToDictionary(e => (char)e, e => new List<string>());
}
