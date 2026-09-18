using GestPlan.Core.Conges;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.Tests;

public class ValidateurAbsenceTests
{
    [Fact]
    public void Valider_RejetteUneAbsenceDontLaFinEstAvantLeDebut()
    {
        var absence = new Absence
        {
            EmployeId = 1,
            DateDebut = new DateOnly(2026, 6, 10),
            DateFin = new DateOnly(2026, 6, 5)
        };

        Assert.NotEmpty(ValidateurAbsence.Valider(absence));
    }

    [Fact]
    public void Valider_AccepteUneAbsenceCoherente()
    {
        var absence = new Absence
        {
            EmployeId = 1,
            DateDebut = new DateOnly(2026, 6, 5),
            DateFin = new DateOnly(2026, 6, 10)
        };

        Assert.Empty(ValidateurAbsence.Valider(absence));
    }

    [Fact]
    public void ChevaucheUneAutreAbsence_DetecteLeChevauchementPourLeMemeEmploye()
    {
        var absence = new Absence { Id = 1, EmployeId = 1, DateDebut = new DateOnly(2026, 6, 5), DateFin = new DateOnly(2026, 6, 10) };
        var autre = new Absence { Id = 2, EmployeId = 1, DateDebut = new DateOnly(2026, 6, 8), DateFin = new DateOnly(2026, 6, 15), Statut = StatutAbsence.Validee };

        Assert.True(ValidateurAbsence.ChevaucheUneAutreAbsence(absence, [autre]));
    }

    [Fact]
    public void ChevaucheUneAutreAbsence_IgnoreLesAbsencesRefusees()
    {
        var absence = new Absence { Id = 1, EmployeId = 1, DateDebut = new DateOnly(2026, 6, 5), DateFin = new DateOnly(2026, 6, 10) };
        var autre = new Absence { Id = 2, EmployeId = 1, DateDebut = new DateOnly(2026, 6, 8), DateFin = new DateOnly(2026, 6, 15), Statut = StatutAbsence.Refusee };

        Assert.False(ValidateurAbsence.ChevaucheUneAutreAbsence(absence, [autre]));
    }

    [Fact]
    public void ChevaucheUneAutreAbsence_IgnoreLesAutresEmployes()
    {
        var absence = new Absence { Id = 1, EmployeId = 1, DateDebut = new DateOnly(2026, 6, 5), DateFin = new DateOnly(2026, 6, 10) };
        var autre = new Absence { Id = 2, EmployeId = 2, DateDebut = new DateOnly(2026, 6, 8), DateFin = new DateOnly(2026, 6, 15), Statut = StatutAbsence.Validee };

        Assert.False(ValidateurAbsence.ChevaucheUneAutreAbsence(absence, [autre]));
    }

    [Fact]
    public void ChevaucheUnCreneauPlanning_DetecteUnCreneauSurLaPeriodeDAbsence()
    {
        var absence = new Absence { EmployeId = 1, DateDebut = new DateOnly(2026, 6, 5), DateFin = new DateOnly(2026, 6, 10) };
        var creneau = new CreneauPlanning { EmployeId = 1, Date = new DateOnly(2026, 6, 7) };

        Assert.True(ValidateurAbsence.ChevaucheUnCreneauPlanning(absence, [creneau]));
    }

    [Fact]
    public void ChevaucheUnCreneauPlanning_NeDetectePasUnCreneauHorsPeriode()
    {
        var absence = new Absence { EmployeId = 1, DateDebut = new DateOnly(2026, 6, 5), DateFin = new DateOnly(2026, 6, 10) };
        var creneau = new CreneauPlanning { EmployeId = 1, Date = new DateOnly(2026, 6, 20) };

        Assert.False(ValidateurAbsence.ChevaucheUnCreneauPlanning(absence, [creneau]));
    }
}
