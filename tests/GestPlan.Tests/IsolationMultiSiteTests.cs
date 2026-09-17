using GestPlan.Core.Entites;
using GestPlan.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GestPlan.Tests;

public class IsolationMultiSiteTests : IDisposable
{
    private readonly SqliteConnection _connexion;
    private readonly GestPlanDbContext _contexte;

    public IsolationMultiSiteTests()
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
    public async Task EmployesDUnSite_NeSontPasVisiblesDepuisUnAutreSite()
    {
        var siteNord = new Site { Nom = "Magasin Nord" };
        var siteSud = new Site { Nom = "Magasin Sud" };
        var employeNord = new Employe { Nom = "Dupont", Prenom = "Alice", SitePrincipal = siteNord };
        var employeSud = new Employe { Nom = "Martin", Prenom = "Bruno", SitePrincipal = siteSud };

        _contexte.AddRange(siteNord, siteSud, employeNord, employeSud);
        await _contexte.SaveChangesAsync();

        var employesDuSiteNord = await _contexte.Employes
            .Where(e => e.SitePrincipalId == siteNord.Id)
            .ToListAsync();

        Assert.Single(employesDuSiteNord);
        Assert.Equal("Dupont", employesDuSiteNord[0].Nom);
    }

    [Fact]
    public async Task ReglesConformite_SontSpecifiquesAChaqueSite()
    {
        var siteA = new Site { Nom = "Magasin A" };
        var siteB = new Site { Nom = "Magasin B" };
        _contexte.AddRange(siteA, siteB);
        await _contexte.SaveChangesAsync();

        var reglesA = new ReglesConformite { Site = siteA, JoursConsecutifsMaximum = 6 };
        var reglesB = new ReglesConformite { Site = siteB, JoursConsecutifsMaximum = 5 };
        _contexte.AddRange(reglesA, reglesB);
        await _contexte.SaveChangesAsync();

        var reglesDuSiteA = await _contexte.ReglesConformite.SingleAsync(r => r.SiteId == siteA.Id);
        var reglesDuSiteB = await _contexte.ReglesConformite.SingleAsync(r => r.SiteId == siteB.Id);

        Assert.Equal(6, reglesDuSiteA.JoursConsecutifsMaximum);
        Assert.Equal(5, reglesDuSiteB.JoursConsecutifsMaximum);
    }

    [Fact]
    public async Task UnSite_NePeutAvoirQuUnSeulJeuDeReglesConformite()
    {
        var site = new Site { Nom = "Magasin Unique" };
        _contexte.Add(site);
        await _contexte.SaveChangesAsync();

        _contexte.Add(new ReglesConformite { Site = site });
        await _contexte.SaveChangesAsync();

        using var autreContexte = new GestPlanDbContext(
            new DbContextOptionsBuilder<GestPlanDbContext>().UseSqlite(_connexion).Options);

        autreContexte.Add(new ReglesConformite { SiteId = site.Id });

        await Assert.ThrowsAnyAsync<Exception>(() => autreContexte.SaveChangesAsync());
    }

    [Fact]
    public async Task EmployeMultiSite_EstRattacheAUnSitePrincipalEtDesSitesSecondaires()
    {
        var sitePrincipal = new Site { Nom = "Site Principal" };
        var siteSecondaire = new Site { Nom = "Site Secondaire" };
        var employe = new Employe { Nom = "Petit", Prenom = "Claire", SitePrincipal = sitePrincipal };

        _contexte.AddRange(sitePrincipal, siteSecondaire, employe);
        await _contexte.SaveChangesAsync();

        _contexte.Add(new EmployeSite { Employe = employe, Site = siteSecondaire });
        await _contexte.SaveChangesAsync();

        var rattachements = await _contexte.EmployesSites
            .Where(es => es.EmployeId == employe.Id)
            .ToListAsync();

        Assert.Single(rattachements);
        Assert.Equal(siteSecondaire.Id, rattachements[0].SiteId);
        Assert.Equal(sitePrincipal.Id, employe.SitePrincipalId);
    }

    public void Dispose()
    {
        _contexte.Dispose();
        _connexion.Dispose();
    }
}
