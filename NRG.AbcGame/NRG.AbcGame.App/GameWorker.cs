using Microsoft.Extensions.Hosting;

namespace NRG.AbcGame.App;

public class GameWorker(IHost host) : BackgroundService
{
    private readonly GameHost _gameHost = new();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await _gameHost.HostGame(ct);
        await host.StopAsync(ct);
    }
}
