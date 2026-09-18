using GestPlan.Core.Entites;

namespace GestPlan.Core.Conformite;

/// <summary>
/// Décrit une anomalie de conformité légale détectée pour un employé sur une période donnée.
/// </summary>
public record AnomalieConformite(
    TypeAnomalieConformite Type,
    int EmployeId,
    DateOnly Date,
    CreneauPlanning? Creneau,
    CreneauPlanning? CreneauSuivant);
