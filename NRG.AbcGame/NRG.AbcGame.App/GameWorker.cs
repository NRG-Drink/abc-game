using Microsoft.Extensions.Hosting;

namespace NRG.AbcGame.App;

public class GameWorker : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => new GameHost().HostGame();
}
