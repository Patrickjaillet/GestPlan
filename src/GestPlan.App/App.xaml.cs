using System.IO;
using System.Windows;
using GestPlan.App.Services;
using GestPlan.App.ViewModels;
using GestPlan.App.Views.Pages;
using GestPlan.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace GestPlan.App;

/// <summary>
/// Point d'entrée de l'application : configure l'hôte générique (DI), Serilog et la fenêtre principale.
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public static IServiceProvider Services { get; private set; } = null!;

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
                services.AjouterGestPlanData();

                services.AddSingleton<IServiceLocalisation, ServiceLocalisation>();

                services.AddNavigationViewPageProvider();
                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddTransient<PlanningPage>();
                services.AddTransient<PlanningViewModel>();
                services.AddTransient<EmployesPage>();
                services.AddTransient<EmployesViewModel>();
                services.AddTransient<CongesPage>();
                services.AddTransient<CongesViewModel>();
                services.AddTransient<RapportsPage>();
                services.AddTransient<RapportsViewModel>();
                services.AddTransient<ParametresPage>();
                services.AddTransient<ParametresViewModel>();
                services.AddTransient<AProposPage>();
                services.AddTransient<AProposViewModel>();
            })
            .Build();

        Services = _host.Services;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        await _host.StartAsync();

        Log.Information("Démarrage de GestPlan");

        using (var portee = _host.Services.CreateScope())
        {
            var initialiseur = portee.ServiceProvider.GetRequiredService<InitialiseurBaseDeDonnees>();
            await initialiseur.InitialiserAsync();
        }

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
