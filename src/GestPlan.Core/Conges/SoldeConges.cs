namespace GestPlan.Core.Conges;

/// <summary>
/// Solde de congés d'un employé pour une année de référence donnée.
/// </summary>
public record SoldeConges(decimal JoursAcquis, decimal JoursPris, decimal JoursRestants)
{
    public static SoldeConges Calculer(decimal joursAcquis, decimal joursPris) =>
        new(joursAcquis, joursPris, joursAcquis - joursPris);
}
