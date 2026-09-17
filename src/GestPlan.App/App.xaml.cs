using System.IO;
using System.Windows;
using GestPlan.App.Services;
using GestPlan.App.ViewModels;
using GestPlan.App.Views.Pages;
using GestPlan.Core.Securite;
using GestPlan.Data;
using GestPlan.Data.Repositories;
using GestPlan.Data.Securite;
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
    private SurveillantInactiviteService? _surveillantInactivite;

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
                services.AddSingleton<ISelecteurSiteService, SelecteurSiteService>();
                services.AddSingleton<ISessionUtilisateurService, SessionUtilisateurService>();

                services.AddSingleton<IServiceHachageMotDePasse, ServiceHachageMotDePasse>();
                services.AddSingleton(new PolitiqueMotDePasse());
                services.AddSingleton<IServiceAuthentification, ServiceAuthentification>();

                services.AddNavigationViewPageProvider();
                services.AddSingleton<INavigationService, NavigationService>();

                services.AddTransient<ConnexionWindow>();
                services.AddTransient<ConnexionViewModel>();
                services.AddTransient<PremierCompteWindow>();
                services.AddTransient<PremierCompteViewModel>();

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
                services.AddTransient<SitesPage>();
                services.AddTransient<SitesViewModel>();
                services.AddTransient<UtilisateursPage>();
                services.AddTransient<UtilisateursViewModel>();
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

        bool aucunUtilisateur;
        using (var portee = _host.Services.CreateScope())
        {
            var initialiseur = portee.ServiceProvider.GetRequiredService<InitialiseurBaseDeDonnees>();
            await initialiseur.InitialiserAsync();

            var unitOfWorkFactory = portee.ServiceProvider.GetRequiredService<IUnitOfWorkFactory>();
            using var unitOfWork = unitOfWorkFactory.Creer();
            aucunUtilisateur = (await unitOfWork.Utilisateurs.ObtenirTousAsync()).Count == 0;
        }

        var session = _host.Services.GetRequiredService<ISessionUtilisateurService>();

        if (aucunUtilisateur)
        {
            var premierCompteWindow = _host.Services.GetRequiredService<PremierCompteWindow>();
            if (premierCompteWindow.ShowDialog() != true)
            {
                Shutdown();
                return;
            }
        }

        var connexionWindow = _host.Services.GetRequiredService<ConnexionWindow>();
        var connexionReussie = connexionWindow.ShowDialog();

        if (connexionReussie != true)
        {
            Shutdown();
            return;
        }

        Log.Information("Connexion de l'utilisateur {NomUtilisateur}", session.UtilisateurConnecte!.NomUtilisateur);

        _surveillantInactivite = new SurveillantInactiviteService(session);
        _surveillantInactivite.Demarrer();

        session.SessionFermee += async (_, utilisateurDeconnecte) =>
        {
            Log.Information("Session fermée pour {NomUtilisateur} (inactivité ou déconnexion)", utilisateurDeconnecte.NomUtilisateur);

            var serviceAuthentification = _host.Services.GetRequiredService<IServiceAuthentification>();
            await serviceAuthentification.DeconnecterAsync(utilisateurDeconnecte.Id);

            Shutdown();
        };

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Closed += (_, _) => Shutdown();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        Log.Information("Fermeture de GestPlan");

        _surveillantInactivite?.Dispose();

        await _host.StopAsync();
        _host.Dispose();
        await Log.CloseAndFlushAsync();

        base.OnExit(e);
    }
}
