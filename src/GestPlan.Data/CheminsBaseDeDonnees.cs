namespace GestPlan.Data;

/// <summary>
/// Résout les chemins du fichier de base de données SQLite et de son dossier de sauvegardes,
/// tous deux situés dans le profil applicatif de l'utilisateur (jamais dans le dossier d'installation).
/// </summary>
public static class CheminsBaseDeDonnees
{
    private const string NomDossierApplication = "GestPlan";
    private const string NomFichierBase = "gestplan.db";

    public static string DossierDonnees =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), NomDossierApplication);

    public static string CheminBase => Path.Combine(DossierDonnees, NomFichierBase);

    public static string DossierSauvegardes => Path.Combine(DossierDonnees, "Sauvegardes");

    public static string ChaineConnexion => $"Data Source={CheminBase}";
}
