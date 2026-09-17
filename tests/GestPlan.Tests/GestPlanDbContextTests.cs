using GestPlan.Core.Entites;
using GestPlan.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GestPlan.Tests;

public class GestPlanDbContextTests : IDisposable
{
    private readonly SqliteConnection _connexion;
    private readonly GestPlanDbContext _contexte;

    public GestPlanDbContextTests()
    {
        _connexion = new SqliteConnection("Data Source=:memory:");
        _connexion.Open();

        var options = new DbContextOptionsBuilder<GestPlanDbContext>()
            .UseSqlite(_connexion)
            .Options;

        _contexte = new GestPlanDbContext(options);
        _contexte.Database.EnsureCreated();
    }

    [Fact]
    public async Task AjouterSite_RenseigneLaDateDeCreation()
    {
        var site = new Site { Nom = "Magasin Centre" };
        await _contexte.Sites.AddAsync(site);

        await _contexte.SaveChangesAsync();

        Assert.NotEqual(default, site.DateCreation);
        Assert.Null(site.DateModification);
    }

    [Fact]
    public async Task AjouterSite_CreeUneEntreeDAudit()
    {
        var site = new Site { Nom = "Magasin Nord" };
        await _contexte.Sites.AddAsync(site);
        await _contexte.SaveChangesAsync();

        var entree = await _contexte.JournauxAudit.SingleAsync(j => j.NomEntite == nameof(Site));

        Assert.Equal(TypeActionAudit.Creation, entree.Action);
        Assert.Equal(site.Id.ToString(), entree.CleEntite);
        Assert.Null(entree.ValeursAvant);
        Assert.NotNull(entree.ValeursApres);
    }

    [Fact]
    public async Task ModifierSite_RenseigneLaDateDeModificationEtAuditeLAvantApres()
    {
        var site = new Site { Nom = "Magasin Sud" };
        await _contexte.Sites.AddAsync(site);
        await _contexte.SaveChangesAsync();

        site.Nom = "Magasin Sud (renommé)";
        await _contexte.SaveChangesAsync();

        Assert.NotNull(site.DateModification);

        var entree = await _contexte.JournauxAudit
            .Where(j => j.NomEntite == nameof(Site) && j.Action == TypeActionAudit.Modification)
            .SingleAsync();

        Assert.Contains("Magasin Sud", entree.ValeursAvant);
        Assert.Contains("renommé", entree.ValeursApres);
    }

    [Fact]
    public async Task SupprimerEmploye_EstImpossibleSansContratOrphelin_ContratEstSupprimeEnCascade()
    {
        var site = new Site { Nom = "Magasin Est" };
        var poste = new Poste { Nom = "Caisse", Site = site };
        var employe = new Employe { Nom = "Martin", Prenom = "Julie", SitePrincipal = site };
        var contrat = new Contrat
        {
            Employe = employe,
            Site = site,
            Poste = poste,
            HeuresHebdomadaires = 35,
            TauxHoraireBrut = 12.5m,
            DateDebut = DateOnly.FromDateTime(DateTime.Today)
        };

        _contexte.AddRange(site, poste, employe, contrat);
        await _contexte.SaveChangesAsync();

        _contexte.Employes.Remove(employe);
        await _contexte.SaveChangesAsync();

        Assert.False(await _contexte.Contrats.AnyAsync(c => c.Id == contrat.Id));
    }

    public void Dispose()
    {
        _contexte.Dispose();
        _connexion.Dispose();
    }
}
