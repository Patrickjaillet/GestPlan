using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;
using GestPlan.Core.Securite;
using GestPlan.Data;
using GestPlan.Data.Repositories;
using GestPlan.Data.Securite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestPlan.Tests;

public class ServiceAuthentificationTests : IDisposable
{
    private readonly SqliteConnection _connexion;
    private readonly ServiceProvider _serviceProvider;

    public ServiceAuthentificationTests()
    {
        _connexion = new SqliteConnection("Data Source=:memory:");
        _connexion.Open();

        var services = new ServiceCollection();
        services.AddDbContext<GestPlanDbContext>(options => options.UseSqlite(_connexion));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddSingleton<IServiceHachageMotDePasse, ServiceHachageMotDePasse>();
        services.AddSingleton(new PolitiqueMotDePasse());
        services.AddSingleton<IServiceAuthentification, ServiceAuthentification>();

        _serviceProvider = services.BuildServiceProvider();

        using var contexte = _serviceProvider.GetRequiredService<GestPlanDbContext>();
        contexte.Database.EnsureCreated();
    }

    private async Task<Utilisateur> CreerUtilisateurAsync(string nomUtilisateur, string motDePasse, RoleUtilisateur role, int? siteAssigneId = null)
    {
        var hachage = _serviceProvider.GetRequiredService<IServiceHachageMotDePasse>();
        using var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWorkFactory>().Creer();

        var utilisateur = new Utilisateur
        {
            NomUtilisateur = nomUtilisateur,
            HashMotDePasse = hachage.Hacher(motDePasse),
            Role = role,
            SiteAssigneId = siteAssigneId
        };

        await unitOfWork.Utilisateurs.AjouterAsync(utilisateur);
        await unitOfWork.EnregistrerAsync();

        return utilisateur;
    }

    [Fact]
    public async Task AuthentifierAsync_ReussitAvecLeBonMotDePasse()
    {
        await CreerUtilisateurAsync("alice", "MotDePasseValide1!", RoleUtilisateur.Consultation);
        var service = _serviceProvider.GetRequiredService<IServiceAuthentification>();

        var resultat = await service.AuthentifierAsync("alice", "MotDePasseValide1!");

        Assert.Equal(StatutAuthentification.Succes, resultat.Statut);
    }

    [Fact]
    public async Task AuthentifierAsync_NePermetAucuneElevationDePrivileges_LeRoleRetourneEstToujoursCeluiStockeEnBase()
    {
        await CreerUtilisateurAsync("bruno", "MotDePasseValide1!", RoleUtilisateur.Consultation);
        var service = _serviceProvider.GetRequiredService<IServiceAuthentification>();

        var resultat = await service.AuthentifierAsync("bruno", "MotDePasseValide1!");

        // Le rôle authentifié ne peut jamais être différent de celui enregistré en base :
        // aucune donnée fournie par l'appelant (nom d'utilisateur, mot de passe) ne permet
        // d'influencer le rôle retourné.
        Assert.Equal(RoleUtilisateur.Consultation, resultat.Utilisateur!.Role);
    }

    [Fact]
    public async Task AuthentifierAsync_EchoueAvecUnMauvaisMotDePasse()
    {
        await CreerUtilisateurAsync("claire", "MotDePasseValide1!", RoleUtilisateur.Admin);
        var service = _serviceProvider.GetRequiredService<IServiceAuthentification>();

        var resultat = await service.AuthentifierAsync("claire", "MauvaisMotDePasse");

        Assert.Equal(StatutAuthentification.IdentifiantsInvalides, resultat.Statut);
        Assert.Null(resultat.Utilisateur);
    }

    [Fact]
    public async Task AuthentifierAsync_EchoueSiLeCompteEstInactif()
    {
        var utilisateur = await CreerUtilisateurAsync("david", "MotDePasseValide1!", RoleUtilisateur.Manager);
        using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWorkFactory>().Creer())
        {
            var entite = await unitOfWork.Utilisateurs.ObtenirParIdAsync(utilisateur.Id);
            entite!.EstActif = false;
            unitOfWork.Utilisateurs.Modifier(entite);
            await unitOfWork.EnregistrerAsync();
        }

        var service = _serviceProvider.GetRequiredService<IServiceAuthentification>();
        var resultat = await service.AuthentifierAsync("david", "MotDePasseValide1!");

        Assert.Equal(StatutAuthentification.CompteInactif, resultat.Statut);
    }

    [Fact]
    public async Task AuthentifierAsync_VerrouilleLeCompteApresPlusieursEchecs()
    {
        await CreerUtilisateurAsync("emma", "MotDePasseValide1!", RoleUtilisateur.Manager);
        var service = _serviceProvider.GetRequiredService<IServiceAuthentification>();

        for (var i = 0; i < 5; i++)
        {
            await service.AuthentifierAsync("emma", "MauvaisMotDePasse");
        }

        var resultat = await service.AuthentifierAsync("emma", "MotDePasseValide1!");

        Assert.Equal(StatutAuthentification.CompteVerrouille, resultat.Statut);
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
        _connexion.Dispose();
    }
}
