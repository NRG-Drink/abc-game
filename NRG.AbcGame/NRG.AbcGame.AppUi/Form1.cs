using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using NRG.AbcGame.AppUi.DataBase;
using NRG.AbcGame.AppUi.Services.Savers;
using NRG.AbcGame.Persistence;

namespace NRG.AbcGame.AppUi;

public partial class Form1 : Form
{
    private static readonly string _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ABC-Game");

    public Form1()
    {
        InitializeComponent();

        var path = Path.Combine(_folder, "DataBase");
        var fullPath = Path.GetFullPath(path);

        var host = Host.CreateDefaultBuilder()
            .UseContentRoot(_folder)
            .ConfigureHostConfiguration(builder =>
            {
                //builder.AddJsonFile("");
            })
            .ConfigureServices((context, services) =>
            {
                services.AddWindowsFormsBlazorWebView();
                services
                    .AddFluentUIComponents()
                    .AddSingleton<ISaver, DbSaver>()
                    //.AddSingleton<FileManager>(e => new(_folder))
                    .AddSqlite<AbcDb>($"Data Source={fullPath}\\Abc.db");
            })
            .Build();

#if DEBUG
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AbcDb>();
        //db.Database.MigrateAsync();
        db.Database.EnsureCreatedAsync();
#endif

        blazorWebView1.HostPage = Path.Combine("wwwroot", "index.html");
        blazorWebView1.Services = host.Services;
        //blazorWebView1.Services = services.BuildServiceProvider();
        blazorWebView1.RootComponents.Add<App>("#app");
    }
}
