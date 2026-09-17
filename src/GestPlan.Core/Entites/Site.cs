namespace GestPlan.Core.Entites;

/// <summary>
/// Un magasin (supérette) géré par l'application. Le modèle multi-site complet
/// (adresse, horaires, fuseau horaire, règles de conformité) est étendu en Phase 2.
/// </summary>
public class Site : EntiteBase
{
    public required string Nom { get; set; }

    public bool EstActif { get; set; } = true;

    public ICollection<Employe> Employes { get; set; } = new List<Employe>();

    public ICollection<Poste> Postes { get; set; } = new List<Poste>();

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();
}
