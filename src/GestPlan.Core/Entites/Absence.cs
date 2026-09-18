using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Entites;

/// <summary>
/// Une demande d'absence d'un employé, sur une période donnée, soumise au workflow
/// de validation (demandée → validée/refusée).
/// </summary>
public class Absence : EntiteBase
{
    public int EmployeId { get; set; }

    public Employe Employe { get; set; } = null!;

    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;

    public int TypeAbsenceId { get; set; }

    public TypeAbsence TypeAbsence { get; set; } = null!;

    public DateOnly DateDebut { get; set; }

    public DateOnly DateFin { get; set; }

    public StatutAbsence Statut { get; set; } = StatutAbsence.Demandee;

    /// <summary>Motif de la demande, ou motif du refus si <see cref="Statut"/> est <see cref="StatutAbsence.Refusee"/>.</summary>
    public string? Motif { get; set; }

    public int? ValideParUtilisateurId { get; set; }

    public Utilisateur? ValideParUtilisateur { get; set; }

    public DateTime? DateValidation { get; set; }

    public int NombreJours => DateFin.DayNumber - DateDebut.DayNumber + 1;
}
