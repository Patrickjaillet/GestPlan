using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace GestPlan.App;

/// <summary>
/// Point d'entrée de l'application : configure l'hôte générique (DI), Serilog et la fenêtre principale.
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        var logsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GestPlan", "logs");
        Directory.CreateDirectory(logsDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.File(
                Path.Combine(logsDirectory, "gestplan-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 31,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        _host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        await _host.StartAsync();

        Log.Information("Démarrage de GestPlan");

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        Log.Information("Fermeture de GestPlan");

        await _host.StopAsync();
        _host.Dispose();
        await Log.CloseAndFlushAsync();

        base.OnExit(e);
    }
}
