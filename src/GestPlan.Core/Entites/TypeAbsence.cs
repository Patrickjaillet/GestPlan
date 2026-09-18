namespace GestPlan.Core.Entites;

/// <summary>
/// Un type d'absence paramétrable (congé payé, RTT, maladie, sans solde, formation...).
/// </summary>
public class TypeAbsence : EntiteBase
{
    public required string Nom { get; set; }

    /// <summary>
    /// Indique si ce type d'absence décompte du solde de congés de l'employé
    /// (ex. congé payé, RTT) ou non (ex. maladie, sans solde, formation).
    /// </summary>
    public bool DecompteDuSolde { get; set; }

    public string? CouleurAffichage { get; set; }

    public bool EstActif { get; set; } = true;
}
