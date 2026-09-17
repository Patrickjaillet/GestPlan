namespace GestPlan.Core.Entites;

/// <summary>
/// Un poste de travail (caisse, rayon, responsable...) rattaché à un site.
/// </summary>
public class Poste : EntiteBase
{
    public required string Nom { get; set; }

    public string? CouleurAffichage { get; set; }

    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();
}
