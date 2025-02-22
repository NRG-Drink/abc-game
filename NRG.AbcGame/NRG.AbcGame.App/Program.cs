using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace NRG.AbcGame.App;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("logging.json", true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Services

                    // Workers
                    services.AddHostedService<GameWorker>();
                })
                .UseSerilog((context, services, LoggerConfiguration) =>
                    LoggerConfiguration.ReadFrom.Configuration(context.Configuration)
                )
                //.UseConsoleLifetime()
                .Build();

            Console.WriteLine("Start host.");
            await host.RunAsync();
            Console.WriteLine("Finish host.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 1;
        }
    }
}
