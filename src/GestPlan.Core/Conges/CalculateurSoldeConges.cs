using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Conges;

/// <summary>
/// Calcule le solde de congés d'un employé pour une période de référence donnée,
/// selon les règles d'acquisition paramétrées pour son site.
/// </summary>
public static class CalculateurSoldeConges
{
    /// <summary>
    /// Calcule le solde de congés sur la période [<paramref name="debutPeriode"/>, <paramref name="finPeriode"/>].
    /// L'acquisition démarre au plus tard à <paramref name="dateDebutAcquisition"/> (ex. date d'entrée de
    /// l'employé) : aucun jour n'est acquis avant cette date même si elle est antérieure à la période.
    /// </summary>
    public static SoldeConges Calculer(
        DateOnly debutPeriode,
        DateOnly finPeriode,
        DateOnly dateDebutAcquisition,
        decimal joursAcquisParMois,
        IEnumerable<Absence> absencesDeLaPeriode)
    {
        var debutEffectif = dateDebutAcquisition > debutPeriode ? dateDebutAcquisition : debutPeriode;

        var moisEcoules = NombreDeMoisEntiers(debutEffectif, finPeriode);
        var joursAcquis = Math.Max(0, moisEcoules) * joursAcquisParMois;

        var joursPris = absencesDeLaPeriode
            .Where(a => a.Statut == StatutAbsence.Validee && a.TypeAbsence.DecompteDuSolde)
            .Sum(a => (decimal)ChevaucherJours(a.DateDebut, a.DateFin, debutPeriode, finPeriode));

        return SoldeConges.Calculer(joursAcquis, joursPris);
    }

    private static int NombreDeMoisEntiers(DateOnly debut, DateOnly fin)
    {
        if (fin < debut)
        {
            return 0;
        }

        var mois = (fin.Year - debut.Year) * 12 + (fin.Month - debut.Month);
        if (fin.Day < debut.Day)
        {
            mois--;
        }

        return mois;
    }

    private static int ChevaucherJours(DateOnly debutA, DateOnly finA, DateOnly debutB, DateOnly finB)
    {
        var debut = debutA > debutB ? debutA : debutB;
        var fin = finA < finB ? finA : finB;

        return fin >= debut ? fin.DayNumber - debut.DayNumber + 1 : 0;
    }
}
