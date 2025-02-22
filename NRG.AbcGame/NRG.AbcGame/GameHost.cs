namespace NRG.AbcGame;

public class GameHost
{
    private readonly GameSettings _settings = new();
    private int _currentLine = Console.GetCursorPosition().Top;

    public async Task HostGame(
        CancellationToken ct = default
        )
    {
        var isReplay = true;
        while (isReplay)
        {
            Console.WriteLine("Let's start a new ABC-Game.");
            var isExit = await _settings.RunMenu(Console.GetCursorPosition().Top);
            if (isExit)
            {
                Console.WriteLine("Thanks for playing the ABC-Game. See you soon.");
                break;
            }

            var game = new Game(_settings);
            var run = await game.RunGame(Console.GetCursorPosition().Top + 1);

            Console.WriteLine("Thanks for playing the ABC-Game.");
            Console.WriteLine("Have a closer look at your results or play again.");
            Console.WriteLine("Press any key to proceed.");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
