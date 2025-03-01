using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using NRG.AbcGame.Persistence;

namespace NRG.AbcGame.AppUi;

public partial class Form1 : Form
{
    private static readonly string _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ABC-Game");
    //private readonly FileManager _saver = new(_folder);

    public Form1()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        services.AddWindowsFormsBlazorWebView();
        services.AddFluentUIComponents();
        services.AddSingleton<FileManager>(e => new(_folder));
        blazorWebView1.HostPage = Path.Combine("wwwroot", "index.html");
        blazorWebView1.Services = services.BuildServiceProvider();
        blazorWebView1.RootComponents.Add<App>("#app");
    }
}
