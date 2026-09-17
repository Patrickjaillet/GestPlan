namespace GestPlan.App.Services;

/// <summary>
/// Gère le stockage des photos d'employés dans le dossier de données de l'application.
/// </summary>
public interface IServicePhotosEmployes
{
    /// <summary>
    /// Copie le fichier source dans le dossier des photos et retourne le chemin relatif
    /// à stocker sur l'entité <see cref="Core.Entites.Employe.CheminPhoto"/>.
    /// </summary>
    string EnregistrerPhoto(string cheminFichierSource);

    /// <summary>
    /// Résout un chemin relatif de photo en chemin absolu, utilisable pour l'affichage.
    /// </summary>
    string ObtenirCheminAbsolu(string cheminRelatif);
}
