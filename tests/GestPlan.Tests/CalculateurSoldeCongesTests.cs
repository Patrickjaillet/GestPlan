using GestPlan.Core.Conges;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.Tests;

public class CalculateurSoldeCongesTests
{
    private static TypeAbsence TypeAvecDecompte() => new() { Nom = "Congé payé", DecompteDuSolde = true };

    private static TypeAbsence TypeSansDecompte() => new() { Nom = "Maladie", DecompteDuSolde = false };

    [Fact]
    public void Calculer_AcquiertLeNombreDeJoursConfigureParMoisEcoule()
    {
        var debutPeriode = new DateOnly(2026, 1, 1);
        var finPeriode = new DateOnly(2026, 6, 1); // 5 mois écoulés
        var dateEntree = new DateOnly(2025, 1, 1); // ancien, n'influence pas le calcul ici

        var solde = CalculateurSoldeConges.Calculer(debutPeriode, finPeriode, dateEntree, 2.5m, []);

        Assert.Equal(12.5m, solde.JoursAcquis);
        Assert.Equal(0m, solde.JoursPris);
        Assert.Equal(12.5m, solde.JoursRestants);
    }

    [Fact]
    public void Calculer_NAcquiertAucunJourAvantLaDateDEntreeDeLEmploye()
    {
        var debutPeriode = new DateOnly(2026, 1, 1);
        var finPeriode = new DateOnly(2026, 6, 1);
        var dateEntree = new DateOnly(2026, 4, 1); // entré en cours de période, seulement 2 mois écoulés

        var solde = CalculateurSoldeConges.Calculer(debutPeriode, finPeriode, dateEntree, 2.5m, []);

        Assert.Equal(5.0m, solde.JoursAcquis);
    }

    [Fact]
    public void Calculer_DeduitLesJoursDAbsenceValideeEtDecomptante()
    {
        var debutPeriode = new DateOnly(2026, 1, 1);
        var finPeriode = new DateOnly(2026, 6, 1);
        var dateEntree = new DateOnly(2025, 1, 1);

        var absence = new Absence
        {
            DateDebut = new DateOnly(2026, 2, 1),
            DateFin = new DateOnly(2026, 2, 5), // 5 jours
            Statut = StatutAbsence.Validee,
            TypeAbsence = TypeAvecDecompte()
        };

        var solde = CalculateurSoldeConges.Calculer(debutPeriode, finPeriode, dateEntree, 2.5m, [absence]);

        Assert.Equal(12.5m, solde.JoursAcquis);
        Assert.Equal(5m, solde.JoursPris);
        Assert.Equal(7.5m, solde.JoursRestants);
    }

    [Fact]
    public void Calculer_IgnoreLesAbsencesNonValideesOuNonDecomptantes()
    {
        var debutPeriode = new DateOnly(2026, 1, 1);
        var finPeriode = new DateOnly(2026, 6, 1);
        var dateEntree = new DateOnly(2025, 1, 1);

        var absenceEnAttente = new Absence
        {
            DateDebut = new DateOnly(2026, 2, 1),
            DateFin = new DateOnly(2026, 2, 5),
            Statut = StatutAbsence.Demandee,
            TypeAbsence = TypeAvecDecompte()
        };

        var absenceMaladie = new Absence
        {
            DateDebut = new DateOnly(2026, 3, 1),
            DateFin = new DateOnly(2026, 3, 3),
            Statut = StatutAbsence.Validee,
            TypeAbsence = TypeSansDecompte()
        };

        var solde = CalculateurSoldeConges.Calculer(debutPeriode, finPeriode, dateEntree, 2.5m, [absenceEnAttente, absenceMaladie]);

        Assert.Equal(0m, solde.JoursPris);
    }
}
