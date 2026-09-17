using System.Text;
using ClosedXML.Excel;
using GestPlan.Core.Entites;
using GestPlan.Data;
using GestPlan.Data.Import;
using GestPlan.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestPlan.Tests;

public class ServiceImportEmployesTests : IDisposable
{
    private readonly SqliteConnection _connexion;
    private readonly ServiceProvider _serviceProvider;
    private readonly ServiceImportEmployes _service;

    public ServiceImportEmployesTests()
    {
        _connexion = new SqliteConnection("Data Source=:memory:");
        _connexion.Open();

        var services = new ServiceCollection();
        services.AddDbContext<GestPlanDbContext>(options => options.UseSqlite(_connexion));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

        _serviceProvider = services.BuildServiceProvider();

        using var contexte = _serviceProvider.GetRequiredService<GestPlanDbContext>();
        contexte.Database.EnsureCreated();
        contexte.Add(new Site { Nom = "Magasin Centre" });
        contexte.SaveChanges();

        _service = new ServiceImportEmployes(_serviceProvider.GetRequiredService<IUnitOfWorkFactory>());
    }

    private static Stream CreerFluxCsv(string contenu) =>
        new MemoryStream(Encoding.UTF8.GetBytes(contenu));

    [Fact]
    public void LireEtValiderCsv_IgnoreLaLigneDEnTete()
    {
        using var flux = CreerFluxCsv("Nom;Prenom;Email;Telephone;Site\nDupont;Alice;alice@test.fr;;Magasin Centre\n");

        var resultat = _service.LireEtValiderCsv(flux);

        Assert.Single(resultat);
        Assert.Equal("Dupont", resultat[0].Nom);
    }

    [Fact]
    public void LireEtValiderCsv_DetecteLesChampsObligatoiresManquants()
    {
        using var flux = CreerFluxCsv("Nom;Prenom;Email;Telephone;Site\n;;;;\n");

        var resultat = _service.LireEtValiderCsv(flux);

        Assert.False(resultat[0].EstValide);
        Assert.Contains(resultat[0].Erreurs, e => e.Contains("nom"));
        Assert.Contains(resultat[0].Erreurs, e => e.Contains("prénom"));
        Assert.Contains(resultat[0].Erreurs, e => e.Contains("site"));
    }

    [Fact]
    public void LireEtValiderCsv_DetecteUneAdresseEmailInvalide()
    {
        using var flux = CreerFluxCsv("Nom;Prenom;Email;Telephone;Site\nMartin;Bruno;pas-un-email;;Magasin Centre\n");

        var resultat = _service.LireEtValiderCsv(flux);

        Assert.False(resultat[0].EstValide);
        Assert.Contains(resultat[0].Erreurs, e => e.Contains("e-mail"));
    }

    [Fact]
    public void LireEtValiderExcel_LitLesLignesDeDonnees()
    {
        using var classeur = new XLWorkbook();
        var feuille = classeur.Worksheets.Add("Employes");
        feuille.Cell(1, 1).Value = "Nom";
        feuille.Cell(1, 2).Value = "Prenom";
        feuille.Cell(1, 3).Value = "Email";
        feuille.Cell(1, 4).Value = "Telephone";
        feuille.Cell(1, 5).Value = "Site";
        feuille.Cell(2, 1).Value = "Petit";
        feuille.Cell(2, 2).Value = "Claire";
        feuille.Cell(2, 5).Value = "Magasin Centre";

        using var flux = new MemoryStream();
        classeur.SaveAs(flux);
        flux.Position = 0;

        var resultat = _service.LireEtValiderExcel(flux);

        Assert.Single(resultat);
        Assert.Equal("Petit", resultat[0].Nom);
        Assert.True(resultat[0].EstValide);
    }

    [Fact]
    public async Task ImporterAsync_CreeLesEmployesPourLesLignesValides()
    {
        var lignes = new[]
        {
            new LigneImportEmploye { NumeroLigne = 2, Nom = "Dupont", Prenom = "Alice", NomSitePrincipal = "Magasin Centre", Erreurs = [] },
            new LigneImportEmploye { NumeroLigne = 3, Nom = "Invalide", Prenom = "X", NomSitePrincipal = "Site Inexistant", Erreurs = [] }
        };

        var nombreImporte = await _service.ImporterAsync(lignes);

        Assert.Equal(1, nombreImporte);

        using var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWorkFactory>().Creer();
        var employes = await unitOfWork.Employes.ObtenirTousAsync();
        Assert.Single(employes);
        Assert.Equal("Dupont", employes[0].Nom);
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
        _connexion.Dispose();
    }
}
