using System.IO;

namespace GestPlan.App.Services;

/// <inheritdoc cref="IServicePhotosEmployes"/>
public class ServicePhotosEmployes : IServicePhotosEmployes
{
    private static readonly string DossierPhotos = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GestPlan", "Photos");

    public string EnregistrerPhoto(string cheminFichierSource)
    {
        Directory.CreateDirectory(DossierPhotos);

        var nomFichier = $"{Guid.NewGuid()}{Path.GetExtension(cheminFichierSource)}";
        var cheminDestination = Path.Combine(DossierPhotos, nomFichier);

        File.Copy(cheminFichierSource, cheminDestination, overwrite: true);

        return nomFichier;
    }

    public string ObtenirCheminAbsolu(string cheminRelatif) =>
        Path.Combine(DossierPhotos, cheminRelatif);
}
