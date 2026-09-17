using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Entites;

/// <summary>
/// Un contrat de travail liant un employé à un site et un poste, sur une période donnée.
/// L'historique des avenants/renouvellements est traité en Phase 4.
/// </summary>
public class Contrat : EntiteBase
{
    public int EmployeId { get; set; }

    public Employe Employe { get; set; } = null!;

    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;

    public int PosteId { get; set; }

    public Poste Poste { get; set; } = null!;

    public TypeContrat Type { get; set; }

    public QuotiteTravail Quotite { get; set; }

    public decimal HeuresHebdomadaires { get; set; }

    public decimal TauxHoraireBrut { get; set; }

    public DateOnly DateDebut { get; set; }

    public DateOnly? DateFin { get; set; }
}
