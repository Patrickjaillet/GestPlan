using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Entites;

/// <summary>
/// Un contrat de travail liant un employé à un site et un poste, sur une période donnée.
/// Un avenant ou un renouvellement est représenté par un nouveau <see cref="Contrat"/>
/// référençant le précédent via <see cref="ContratPrecedentId"/>, formant une chaîne
/// d'historique complète pour l'employé.
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

    public int? ContratPrecedentId { get; set; }

    public Contrat? ContratPrecedent { get; set; }
}
