namespace GestPlan.App.Services;

/// <summary>
/// Gère le site actuellement sélectionné dans l'application, persisté entre les sessions.
/// </summary>
public interface ISelecteurSiteService
{
    int? SiteSelectionneId { get; }

    event EventHandler<int?>? SiteChange;

    void SelectionnerSite(int? siteId);
}
