namespace GestPlan.Data.Import;

/// <summary>
/// Importe des employés en masse depuis un fichier CSV ou Excel, avec validation
/// des données avant toute écriture en base.
/// </summary>
public interface IServiceImportEmployes
{
    /// <summary>
    /// Analyse et valide le contenu d'un fichier CSV (encodage UTF-8, séparateur point-virgule,
    /// colonnes attendues : Nom;Prenom;Email;Telephone;Site).
    /// </summary>
    IReadOnlyList<LigneImportEmploye> LireEtValiderCsv(Stream fluxCsv);

    /// <summary>
    /// Analyse et valide le contenu d'un classeur Excel (première feuille, même colonnes que le CSV,
    /// première ligne = en-têtes).
    /// </summary>
    IReadOnlyList<LigneImportEmploye> LireEtValiderExcel(Stream fluxExcel);

    /// <summary>
    /// Crée en base les employés correspondant aux lignes valides fournies. Les lignes invalides
    /// sont ignorées ; l'appelant est responsable de les avoir filtrées ou signalées au préalable.
    /// </summary>
    Task<int> ImporterAsync(IEnumerable<LigneImportEmploye> lignesValides, CancellationToken cancellationToken = default);
}
