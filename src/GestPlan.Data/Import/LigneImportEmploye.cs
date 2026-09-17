namespace GestPlan.Data.Import;

/// <summary>
/// Résultat de la validation d'une ligne d'import d'employé (CSV ou Excel).
/// </summary>
public class LigneImportEmploye
{
    public int NumeroLigne { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public string? NomSitePrincipal { get; set; }

    public IReadOnlyList<string> Erreurs { get; set; } = [];

    public bool EstValide => Erreurs.Count == 0;
}
