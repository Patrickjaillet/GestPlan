using GestPlan.Core.Conformite;
using GestPlan.Core.Entites;

namespace GestPlan.Tests;

public class MoteurConformiteTests
{
    private static CreneauPlanning Creneau(int employeId, DateOnly date, TimeOnly debut, TimeOnly fin) => new()
    {
        EmployeId = employeId,
        SiteId = 1,
        PosteId = 1,
        Date = date,
        HeureDebut = debut,
        HeureFin = fin
    };

    private static ReglesConformite ReglesParDefaut() => new()
    {
        SiteId = 1,
        ReposQuotidienMinimum = TimeSpan.FromHours(11),
        ReposHebdomadaireMinimum = TimeSpan.FromHours(35),
        DureeMaximaleQuotidienne = TimeSpan.FromHours(10),
        DureeMaximaleHebdomadaire = TimeSpan.FromHours(48),
        DureeTravailContinuAvantPause = TimeSpan.FromHours(6),
        DureePauseObligatoire = TimeSpan.FromMinutes(20),
        JoursConsecutifsMaximum = 6
    };

    [Fact]
    public void Evaluer_NeDetectePasDAnomalieSurUnPlanningConforme()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(9, 0), new TimeOnly(13, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.Empty(anomalies);
    }

    [Fact]
    public void Evaluer_DetecteUnReposQuotidienInsuffisant()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(9, 0), new TimeOnly(20, 0)),
            Creneau(1, new DateOnly(2026, 9, 22), new TimeOnly(6, 0), new TimeOnly(14, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.Contains(anomalies, a => a.Type == TypeAnomalieConformite.ReposQuotidienInsuffisant);
    }

    [Fact]
    public void Evaluer_AccepteUnReposQuotidienExactementAuMinimum()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(8, 0), new TimeOnly(17, 0)),
            Creneau(1, new DateOnly(2026, 9, 22), new TimeOnly(4, 0), new TimeOnly(10, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.DoesNotContain(anomalies, a => a.Type == TypeAnomalieConformite.ReposQuotidienInsuffisant);
    }

    [Fact]
    public void Evaluer_DetecteUneDureeQuotidienneDepassee()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(6, 0), new TimeOnly(12, 0)),
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(13, 0), new TimeOnly(18, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.Contains(anomalies, a => a.Type == TypeAnomalieConformite.DureeQuotidienneDepassee);
    }

    [Fact]
    public void Evaluer_NeDetectePasDeDureeQuotidienneDepasseeSousLeMaximum()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(9, 0), new TimeOnly(17, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.DoesNotContain(anomalies, a => a.Type == TypeAnomalieConformite.DureeQuotidienneDepassee);
    }

    [Fact]
    public void Evaluer_DetecteUnePauseObligatoireManquanteApresUneLonguePeriodeContinue()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(8, 0), new TimeOnly(14, 30))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.Contains(anomalies, a => a.Type == TypeAnomalieConformite.PauseObligatoireManquante);
    }

    [Fact]
    public void Evaluer_DetectePlusieursJoursAvecPauseManquanteDansLaMemePeriode()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 7), new TimeOnly(8, 0), new TimeOnly(19, 0)),
            Creneau(1, new DateOnly(2026, 9, 14), new TimeOnly(8, 0), new TimeOnly(19, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles)
            .Where(a => a.Type == TypeAnomalieConformite.PauseObligatoireManquante)
            .ToList();

        Assert.Equal(2, anomalies.Count);
        Assert.Contains(anomalies, a => a.Date == new DateOnly(2026, 9, 7));
        Assert.Contains(anomalies, a => a.Date == new DateOnly(2026, 9, 14));
    }

    [Fact]
    public void Evaluer_AccepteUnePauseSuffisanteEntreDeuxPeriodesDeTravail()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(8, 0), new TimeOnly(14, 0)),
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(14, 30), new TimeOnly(18, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.DoesNotContain(anomalies, a => a.Type == TypeAnomalieConformite.PauseObligatoireManquante);
    }

    [Fact]
    public void Evaluer_DetecteUneDureeHebdomadaireDepassee()
    {
        var regles = ReglesParDefaut();
        var creneaux = new List<CreneauPlanning>();
        var lundi = new DateOnly(2026, 9, 21);
        for (var i = 0; i < 6; i++)
        {
            creneaux.Add(Creneau(1, lundi.AddDays(i), new TimeOnly(8, 0), new TimeOnly(18, 0)));
        }

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.Contains(anomalies, a => a.Type == TypeAnomalieConformite.DureeHebdomadaireDepassee);
    }

    [Fact]
    public void Evaluer_AccepteUneDureeHebdomadaireSousLeMaximum()
    {
        var regles = ReglesParDefaut();
        var lundi = new DateOnly(2026, 9, 21);
        var creneaux = new[]
        {
            Creneau(1, lundi, new TimeOnly(9, 0), new TimeOnly(13, 0)),
            Creneau(1, lundi.AddDays(1), new TimeOnly(9, 0), new TimeOnly(13, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.DoesNotContain(anomalies, a => a.Type == TypeAnomalieConformite.DureeHebdomadaireDepassee);
    }

    [Fact]
    public void Evaluer_DetecteUnReposHebdomadaireInsuffisant()
    {
        var regles = ReglesParDefaut();
        var lundi = new DateOnly(2026, 9, 21);
        var creneaux = new List<CreneauPlanning>();
        for (var i = 0; i < 7; i++)
        {
            creneaux.Add(Creneau(1, lundi.AddDays(i), new TimeOnly(9, 0), new TimeOnly(13, 0)));
        }

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.Contains(anomalies, a => a.Type == TypeAnomalieConformite.ReposHebdomadaireInsuffisant);
    }

    [Fact]
    public void Evaluer_AccepteUneSemaineAvecUnJourCompletDeRepos()
    {
        var regles = ReglesParDefaut();
        var lundi = new DateOnly(2026, 9, 21);
        var creneaux = new[]
        {
            Creneau(1, lundi, new TimeOnly(9, 0), new TimeOnly(17, 0)),
            Creneau(1, lundi.AddDays(1), new TimeOnly(9, 0), new TimeOnly(17, 0)),
            Creneau(1, lundi.AddDays(2), new TimeOnly(9, 0), new TimeOnly(17, 0)),
            Creneau(1, lundi.AddDays(3), new TimeOnly(9, 0), new TimeOnly(17, 0)),
            Creneau(1, lundi.AddDays(4), new TimeOnly(9, 0), new TimeOnly(17, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.DoesNotContain(anomalies, a => a.Type == TypeAnomalieConformite.ReposHebdomadaireInsuffisant);
    }

    [Fact]
    public void Evaluer_DetecteUnDepassementDeJoursConsecutifs()
    {
        var regles = ReglesParDefaut();
        var lundi = new DateOnly(2026, 9, 21);
        var creneaux = new List<CreneauPlanning>();
        for (var i = 0; i < 7; i++)
        {
            creneaux.Add(Creneau(1, lundi.AddDays(i), new TimeOnly(9, 0), new TimeOnly(12, 0)));
        }

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.Contains(anomalies, a => a.Type == TypeAnomalieConformite.JoursConsecutifsDepasses);
    }

    [Fact]
    public void Evaluer_AccepteExactementLeNombreMaximumDeJoursConsecutifs()
    {
        var regles = ReglesParDefaut();
        var lundi = new DateOnly(2026, 9, 21);
        var creneaux = new List<CreneauPlanning>();
        for (var i = 0; i < 6; i++)
        {
            creneaux.Add(Creneau(1, lundi.AddDays(i), new TimeOnly(9, 0), new TimeOnly(12, 0)));
        }

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.DoesNotContain(anomalies, a => a.Type == TypeAnomalieConformite.JoursConsecutifsDepasses);
    }

    [Fact]
    public void Evaluer_IgnoreLesCreneauxDunAutreEmploye()
    {
        var regles = ReglesParDefaut();
        var creneaux = new[]
        {
            Creneau(1, new DateOnly(2026, 9, 21), new TimeOnly(9, 0), new TimeOnly(13, 0)),
            Creneau(2, new DateOnly(2026, 9, 21), new TimeOnly(20, 0), new TimeOnly(23, 0)),
            Creneau(2, new DateOnly(2026, 9, 22), new TimeOnly(0, 0), new TimeOnly(4, 0))
        };

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.Empty(anomalies);
    }

    [Fact]
    public void Evaluer_DeuxSeriesDeJoursConsecutifsSeparesParUneCoupureNeDeclenchentPasDAnomalie()
    {
        var regles = ReglesParDefaut();
        var lundi = new DateOnly(2026, 9, 21);
        var creneaux = new List<CreneauPlanning>();
        for (var i = 0; i < 5; i++)
        {
            creneaux.Add(Creneau(1, lundi.AddDays(i), new TimeOnly(9, 0), new TimeOnly(12, 0)));
        }
        for (var i = 7; i < 12; i++)
        {
            creneaux.Add(Creneau(1, lundi.AddDays(i), new TimeOnly(9, 0), new TimeOnly(12, 0)));
        }

        var anomalies = MoteurConformite.Evaluer(1, creneaux, regles);

        Assert.DoesNotContain(anomalies, a => a.Type == TypeAnomalieConformite.JoursConsecutifsDepasses);
    }
}
