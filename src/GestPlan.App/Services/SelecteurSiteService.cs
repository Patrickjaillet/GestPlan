using System.IO;
using System.Text.Json;

namespace GestPlan.App.Services;

/// <inheritdoc cref="ISelecteurSiteService"/>
public class SelecteurSiteService : ISelecteurSiteService
{
    private static readonly string CheminFichier = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GestPlan", "site-selectionne.json");

    private int? _siteSelectionneId;

    public int? SiteSelectionneId => _siteSelectionneId;

    public event EventHandler<int?>? SiteChange;

    public SelecteurSiteService()
    {
        _siteSelectionneId = ChargerDepuisDisque();
    }

    public void SelectionnerSite(int? siteId)
    {
        if (_siteSelectionneId == siteId)
        {
            return;
        }

        _siteSelectionneId = siteId;
        EnregistrerSurDisque(siteId);
        SiteChange?.Invoke(this, siteId);
    }

    private static int? ChargerDepuisDisque()
    {
        if (!File.Exists(CheminFichier))
        {
            return null;
        }

        var contenu = File.ReadAllText(CheminFichier);
        var donnees = JsonSerializer.Deserialize<PreferenceSite>(contenu);
        return donnees?.SiteSelectionneId;
    }

    private static void EnregistrerSurDisque(int? siteId)
    {
        var dossier = Path.GetDirectoryName(CheminFichier)!;
        Directory.CreateDirectory(dossier);

        var contenu = JsonSerializer.Serialize(new PreferenceSite { SiteSelectionneId = siteId });
        File.WriteAllText(CheminFichier, contenu);
    }

    private sealed class PreferenceSite
    {
        public int? SiteSelectionneId { get; set; }
    }
}
