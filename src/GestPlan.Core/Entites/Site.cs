namespace GestPlan.Core.Entites;

/// <summary>
/// Un magasin (supérette) géré par l'application.
/// </summary>
public class Site : EntiteBase
{
    public required string Nom { get; set; }

    public string? Adresse { get; set; }

    public string? CodePostal { get; set; }

    public string? Ville { get; set; }

    public TimeOnly? HeureOuverture { get; set; }

    public TimeOnly? HeureFermeture { get; set; }

    public string FuseauHoraire { get; set; } = "Europe/Paris";

    public bool EstActif { get; set; } = true;

    public ICollection<Employe> Employes { get; set; } = new List<Employe>();

    public ICollection<EmployeSite> SitesSecondaires { get; set; } = new List<EmployeSite>();

    public ICollection<Poste> Postes { get; set; } = new List<Poste>();

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();

    public ReglesConformite? ReglesConformite { get; set; }
}
