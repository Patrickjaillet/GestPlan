namespace GestPlan.Core.Entites;

/// <summary>
/// Règles d'acquisition automatique des droits à congés payés, paramétrables par site.
/// </summary>
public class ReglesAcquisitionConges : EntiteBase
{
    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;

    /// <summary>Nombre de jours de congés acquis par mois travaillé (ex. 2.5 jours/mois = 30 jours/an).</summary>
    public decimal JoursAcquisParMois { get; set; } = 2.5m;

    /// <summary>Mois de début de la période de référence pour le calcul du solde annuel (1 = janvier).</summary>
    public int MoisDebutPeriodeReference { get; set; } = 6;
}
