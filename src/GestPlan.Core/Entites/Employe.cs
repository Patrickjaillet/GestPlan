namespace GestPlan.Core.Entites;

/// <summary>
/// Un employé de la supérette. La gestion multi-site (rattachement principal +
/// sites secondaires) est étendue en Phase 2.
/// </summary>
public class Employe : EntiteBase
{
    public required string Nom { get; set; }

    public required string Prenom { get; set; }

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public bool EstArchive { get; set; }

    public int SitePrincipalId { get; set; }

    public Site SitePrincipal { get; set; } = null!;

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();
}
