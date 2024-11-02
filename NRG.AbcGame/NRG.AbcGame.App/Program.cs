namespace NRG.AbcGame.App;

internal class Program
{
    static async Task Main(string[] args)
    {
        var interactor = new AbcGame();

        await interactor.AskTime();
        await interactor.AskForTopic();
        interactor.StartGame();

        while (!interactor.Token.IsCancellationRequested)
        {
            await interactor.PrintScreen();
            await interactor.AskForValue();
        }

        await interactor.PrintEnd();
    }
}
