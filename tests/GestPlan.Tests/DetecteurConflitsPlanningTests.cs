using GestPlan.Core.Entites;
using GestPlan.Core.Planning;

namespace GestPlan.Tests;

public class DetecteurConflitsPlanningTests
{
    private static CreneauPlanning Creneau(int id, int employeId, DateOnly date, TimeOnly debut, TimeOnly fin) => new()
    {
        Id = id,
        EmployeId = employeId,
        SiteId = 1,
        PosteId = 1,
        Date = date,
        HeureDebut = debut,
        HeureFin = fin
    };

    [Fact]
    public void Detecter_SignaleUneDoubleAffectationEnCasDeChevauchement()
    {
        var date = new DateOnly(2026, 9, 21);
        var creneau = Creneau(1, employeId: 1, date, new TimeOnly(9, 0), new TimeOnly(13, 0));
        var autre = Creneau(2, employeId: 1, date, new TimeOnly(12, 0), new TimeOnly(17, 0));

        var conflits = DetecteurConflitsPlanning.Detecter(creneau, [autre], []);

        Assert.Single(conflits);
        Assert.Equal(TypeConflitPlanning.DoubleAffectation, conflits[0].Type);
    }

    [Fact]
    public void Detecter_NeSignalePasDeConflitPourDesCreneauxConsecutifs()
    {
        var date = new DateOnly(2026, 9, 21);
        var creneau = Creneau(1, employeId: 1, date, new TimeOnly(9, 0), new TimeOnly(13, 0));
        var autre = Creneau(2, employeId: 1, date, new TimeOnly(13, 0), new TimeOnly(17, 0));

        var conflits = DetecteurConflitsPlanning.Detecter(creneau, [autre], []);

        Assert.Empty(conflits);
    }

    [Fact]
    public void Detecter_NeSignalePasDeConflitPourDesEmployesDifferents()
    {
        var date = new DateOnly(2026, 9, 21);
        var creneau = Creneau(1, employeId: 1, date, new TimeOnly(9, 0), new TimeOnly(13, 0));
        var autre = Creneau(2, employeId: 2, date, new TimeOnly(9, 0), new TimeOnly(13, 0));

        var conflits = DetecteurConflitsPlanning.Detecter(creneau, [autre], []);

        Assert.Empty(conflits);
    }

    [Fact]
    public void Detecter_NeSignalePasDeConflitPourDesJoursDifferents()
    {
        var creneau = Creneau(1, employeId: 1, new DateOnly(2026, 9, 21), new TimeOnly(9, 0), new TimeOnly(13, 0));
        var autre = Creneau(2, employeId: 1, new DateOnly(2026, 9, 22), new TimeOnly(9, 0), new TimeOnly(13, 0));

        var conflits = DetecteurConflitsPlanning.Detecter(creneau, [autre], []);

        Assert.Empty(conflits);
    }

    [Fact]
    public void Detecter_SignaleUneAffectationSurUneIndisponibilite()
    {
        var lundi = new DateOnly(2026, 9, 21); // un lundi
        var creneau = Creneau(1, employeId: 1, lundi, new TimeOnly(9, 0), new TimeOnly(13, 0));
        var indisponibilite = new Indisponibilite
        {
            EmployeId = 1,
            JourSemaine = DayOfWeek.Monday,
            HeureDebut = new TimeOnly(8, 0),
            HeureFin = new TimeOnly(12, 0)
        };

        var conflits = DetecteurConflitsPlanning.Detecter(creneau, [], [indisponibilite]);

        Assert.Single(conflits);
        Assert.Equal(TypeConflitPlanning.Indisponibilite, conflits[0].Type);
    }

    [Fact]
    public void Detecter_NeSignalePasDeConflitSiLIndisponibiliteEstUnAutreJour()
    {
        var lundi = new DateOnly(2026, 9, 21);
        var creneau = Creneau(1, employeId: 1, lundi, new TimeOnly(9, 0), new TimeOnly(13, 0));
        var indisponibilite = new Indisponibilite
        {
            EmployeId = 1,
            JourSemaine = DayOfWeek.Tuesday,
            HeureDebut = new TimeOnly(8, 0),
            HeureFin = new TimeOnly(12, 0)
        };

        var conflits = DetecteurConflitsPlanning.Detecter(creneau, [], [indisponibilite]);

        Assert.Empty(conflits);
    }
}

public class ValidateurCreneauTests
{
    [Fact]
    public void Valider_RejetteUnCreneauDontLaFinEstAvantLeDebut()
    {
        var creneau = new CreneauPlanning
        {
            EmployeId = 1,
            SiteId = 1,
            PosteId = 1,
            Date = new DateOnly(2026, 9, 21),
            HeureDebut = new TimeOnly(13, 0),
            HeureFin = new TimeOnly(9, 0)
        };

        var erreurs = ValidateurCreneau.Valider(creneau);

        Assert.NotEmpty(erreurs);
    }

    [Fact]
    public void Valider_AccepteUnCreneauCoherent()
    {
        var creneau = new CreneauPlanning
        {
            EmployeId = 1,
            SiteId = 1,
            PosteId = 1,
            Date = new DateOnly(2026, 9, 21),
            HeureDebut = new TimeOnly(9, 0),
            HeureFin = new TimeOnly(13, 0)
        };

        Assert.Empty(ValidateurCreneau.Valider(creneau));
    }

    [Fact]
    public void EstVerrouille_RetourneVraiPourUnCreneauPasse()
    {
        var creneau = new CreneauPlanning { Date = new DateOnly(2026, 1, 1) };

        Assert.True(ValidateurCreneau.EstVerrouille(creneau, new DateOnly(2026, 9, 18)));
    }

    [Fact]
    public void EstVerrouille_RetourneFauxPourUnCreneauAujourdhuiOuFutur()
    {
        var aujourdHui = new DateOnly(2026, 9, 18);
        var creneauAujourdhui = new CreneauPlanning { Date = aujourdHui };
        var creneauFutur = new CreneauPlanning { Date = aujourdHui.AddDays(1) };

        Assert.False(ValidateurCreneau.EstVerrouille(creneauAujourdhui, aujourdHui));
        Assert.False(ValidateurCreneau.EstVerrouille(creneauFutur, aujourdHui));
    }
}
