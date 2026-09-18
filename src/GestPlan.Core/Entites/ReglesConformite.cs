namespace GestPlan.Core.Entites;

/// <summary>
/// Paramètres de conformité légale applicables à un site (repos, durées maximales, pauses).
/// Le moteur de règles exploitant ces paramètres est implémenté en Phase 7.
/// </summary>
public class ReglesConformite : EntiteBase
{
    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;

    public TimeSpan ReposQuotidienMinimum { get; set; } = TimeSpan.FromHours(11);

    public TimeSpan ReposHebdomadaireMinimum { get; set; } = TimeSpan.FromHours(35);

    public TimeSpan DureeMaximaleQuotidienne { get; set; } = TimeSpan.FromHours(10);

    public TimeSpan DureeMaximaleHebdomadaire { get; set; } = TimeSpan.FromHours(48);

    public TimeSpan DureeTravailContinuAvantPause { get; set; } = TimeSpan.FromHours(6);

    public TimeSpan DureePauseObligatoire { get; set; } = TimeSpan.FromMinutes(20);

    public int JoursConsecutifsMaximum { get; set; } = 6;

    /// <summary>
    /// Si vrai, la publication d'un planning contenant au moins une anomalie de conformité
    /// non résolue est bloquée pour ce site (voir <c>PlanningViewModel.PublierSemaineAsync</c>).
    /// </summary>
    public bool BloquerPublicationSiNonConforme { get; set; }
}
