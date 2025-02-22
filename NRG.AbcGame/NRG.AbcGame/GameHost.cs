using NRG.AbcGame.Persistence;

namespace NRG.AbcGame;

public class GameHost
{
    private static readonly string _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ABC-Game");
    private readonly GameSettings _settings = new(_folder);
    private readonly FileSaver _saver = new(_folder);

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

            await _saver.Save(run);

            Console.WriteLine("Thanks for playing the ABC-Game.");
            Console.WriteLine("Have a closer look at your results or play again.");
            Console.WriteLine("Press any key to proceed.");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
