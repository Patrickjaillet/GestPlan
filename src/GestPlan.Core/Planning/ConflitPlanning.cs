using GestPlan.Core.Entites;

namespace GestPlan.Core.Planning;

/// <summary>
/// Nature d'un conflit détecté entre deux créneaux de planning.
/// </summary>
public enum TypeConflitPlanning
{
    /// <summary>Le même employé a deux créneaux qui se chevauchent dans le temps.</summary>
    DoubleAffectation,

    /// <summary>Le créneau tombe sur une période d'indisponibilité récurrente déclarée par l'employé.</summary>
    Indisponibilite
}

/// <summary>
/// Décrit un conflit détecté impliquant un créneau de planning.
/// </summary>
public record ConflitPlanning(TypeConflitPlanning Type, CreneauPlanning Creneau, CreneauPlanning? CreneauEnConflit);
