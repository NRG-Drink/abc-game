using NRG.AbcGame.Persistence;

namespace NRG.AbcGame;

public class GameHost
{
    private static readonly string _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ABC-Game");
    private readonly FileManager _saver = new(_folder);

    public async Task HostGame(
        CancellationToken ct = default
        )
    {
        var gameSettings = await _saver.LoadSettingsOrDefaultAsync();
        var menu = new GameMenu(_folder, gameSettings, _saver.SaveSettings);

        var isReplay = true;
        while (isReplay)
        {
            Console.WriteLine("Let's start a new ABC-Game.");
            var (isExit, settings) = await menu.RunMenu(Console.GetCursorPosition().Top);
            if (isExit)
            {
                Console.WriteLine("Thanks for playing the ABC-Game. See you soon.");
                break;
            }

            var game = new Game(settings);
            var run = await game.RunGame(Console.GetCursorPosition().Top + 1);

            var isSave = !run.IsCancelled;
            if (run.IsCancelled)
            {
                Console.Write("Do you want to save the cancelled game? (y|n)  ");
                var saveInput = Console.ReadKey();
                if (saveInput.Key is ConsoleKey.Y)
                {
                    isSave = true;
                }
            }

            if (isSave)
            {
                await _saver.SaveAsync(run);
                Console.WriteLine("\nYour run has been saved.");
            }
            else
            {
                Console.WriteLine("\nYour run was not saved.");
            }

            Console.WriteLine("Thanks for playing the ABC-Game.");
            Console.WriteLine("Have a closer look at your results or play again.");
            Console.WriteLine("Press any key to proceed.");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
