using System.Diagnostics;
using System.Text;

namespace NRG.AbcGame;

public class AbcGame
{
    private readonly Task _timeTask;
    private readonly Stopwatch _stopwatch = new();
    private CancellationTokenSource _cts = new();
    private TimeSpan _time = TimeSpan.FromSeconds(10);
    private int _currentLine = Console.GetCursorPosition().Top - 1;
    private int _gameLine;

    public CancellationToken Token => _cts.Token;
    public string Topic { get; private set; } = string.Empty;
    public Dictionary<char, List<string>> Values { get; private set; } = InitValues();

    public AbcGame()
    {
        _timeTask = new Task(async () =>
        {
            while (!Token.IsCancellationRequested)
            {
                await Task.Delay(300);
                await WriteTime(_stopwatch.Elapsed);
            }
        });
    }

    public async Task AskTime()
    {
        _currentLine++;
        Console.SetCursorPosition(0, _currentLine);
        Console.Write($"Please enter time ({_time:hh\\:mm\\:ss}/{_time.TotalSeconds}s): ");
        var readTime = await Console.In.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(readTime))
        {
            return;
        }

        if (readTime.EndsWith('s'))
        {
            var seconds = int.Parse(readTime[..^1]);
            _time = TimeSpan.FromSeconds(seconds);
        }
        else
        {
            _time = TimeSpan.Parse(readTime);
        }
    }

    public async Task AskForTopic(string defaultTopic = "")
    {
        _currentLine++;
        Console.SetCursorPosition(0, _currentLine);
        Console.Write("Please enter topic: ");
        var topic = await Console.In.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(topic ?? defaultTopic))
        {
            await AskForTopic(defaultTopic);
        }

        Topic = topic ?? defaultTopic;
    }

    public void StartGame()
    {
        _gameLine = ++_currentLine;
        _stopwatch.Restart();
        _cts = new(_time);
        _timeTask.Start();
    }

    public async Task AskForValue()
    {
        Console.SetCursorPosition(0, _gameLine);
        Console.Write($"Please enter value: ");
        var val = await Console.In.ReadLineAsync(Token);
        if (!string.IsNullOrWhiteSpace(val))
        {
            Console.SetCursorPosition(0, _gameLine);
            var replace = Enumerable.Repeat(' ', val.Length);
            Console.Write($"Please enter value: {new string(replace.ToArray())}");
            AddValue(val);
        }

        ToEndLine();
    }

    public async Task PrintScreen()
    {
        Console.SetCursorPosition(0, _gameLine + 1);
        var text = Values
            .Aggregate(
                new StringBuilder(),
                (acc, e) => acc.AppendLine($"{e.Key} ({e.Value.Count}) - {string.Join(", ", e.Value)}")
            )
            .ToString();

        await Console.Out.WriteLineAsync(text);
        ToEndLine();
    }

    public async Task WriteTime(TimeSpan time)
    {
        if (_cts.IsCancellationRequested)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }

        var (l, t) = Console.GetCursorPosition();
        Console.SetCursorPosition(50, _gameLine);
        await Console.Out.WriteLineAsync($"time: {time:hh\\:mm\\:ss\\.fff}");
        Console.SetCursorPosition(l, t);

        if (_cts.IsCancellationRequested)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    public Task PrintEnd() 
        => Console.Out.WriteLineAsync($"Sorry, time is up. ({_time})\n" +
            $"{Values.Aggregate(0, (acc, e) => acc += e.Value.Count)} is your total number of values.");

    private void AddValue(string val)
    {
        if (_cts.IsCancellationRequested)
        {
            return;
        }

        var c = char.ToUpper(val[0]);
        if (Values.TryGetValue(c, out var value))
        {
            Values[c] = value.Union([val.ToLower()]).ToList();
        }
    }

    private void ToEndLine() 
        => Console.SetCursorPosition(0, _gameLine + Values.Count + 1);

    private static Dictionary<char, List<string>> InitValues()
        => Enumerable.Range(65, 26)
            .ToDictionary(e => (char)e, e => new List<string>());
}