namespace GestPlan.Core.Entites;

/// <summary>
/// Un employé de la supérette.
/// </summary>
public class Employe : EntiteBase
{
    public required string Nom { get; set; }

    public required string Prenom { get; set; }

    public DateOnly? DateNaissance { get; set; }

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public string? Adresse { get; set; }

    public string? CodePostal { get; set; }

    public string? Ville { get; set; }

    /// <summary>
    /// Chemin du fichier de la photo, relatif au dossier de données de l'application. <c>null</c> si aucune photo.
    /// </summary>
    public string? CheminPhoto { get; set; }

    public bool EstArchive { get; set; }

    public int SitePrincipalId { get; set; }

    public Site SitePrincipal { get; set; } = null!;

    public ICollection<EmployeSite> SitesSecondaires { get; set; } = new List<EmployeSite>();

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();

    public ICollection<EmployePoste> PostesAutorises { get; set; } = new List<EmployePoste>();

    public ICollection<Indisponibilite> Indisponibilites { get; set; } = new List<Indisponibilite>();
}
