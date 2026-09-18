namespace GestPlan.Core.Conformite;

/// <summary>
/// Libellés français des <see cref="TypeAnomalieConformite"/>, partagés entre l'écran
/// de planning (alertes en temps réel) et l'écran de rapports (export).
/// </summary>
public static class LibellesAnomalieConformite
{
    public static string Obtenir(TypeAnomalieConformite type) => type switch
    {
        TypeAnomalieConformite.ReposQuotidienInsuffisant => "Repos quotidien insuffisant.",
        TypeAnomalieConformite.ReposHebdomadaireInsuffisant => "Repos hebdomadaire insuffisant.",
        TypeAnomalieConformite.DureeQuotidienneDepassee => "Durée maximale quotidienne dépassée.",
        TypeAnomalieConformite.DureeHebdomadaireDepassee => "Durée maximale hebdomadaire dépassée.",
        TypeAnomalieConformite.PauseObligatoireManquante => "Pause obligatoire manquante.",
        TypeAnomalieConformite.JoursConsecutifsDepasses => "Nombre de jours consécutifs travaillés dépassé.",
        _ => "Anomalie de conformité."
    };
}
