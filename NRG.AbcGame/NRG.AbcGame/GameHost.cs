namespace NRG.AbcGame;

public class GameHost
{
    private readonly AbcGame _game = new();

    public async Task<(string, IDictionary<char, List<string>>)> HostGame()
    {
        await _game.AskTime();
        await _game.AskForTopic();
        _game.StartGame();

        while (!_game.Token.IsCancellationRequested)
        {
            await _game.PrintScreen();
            await _game.AskForValue();
        }

        await _game.PrintEnd();

        return (_game.Topic, _game.Values);
    }
}
