namespace GestPlan.Core.Conformite;

/// <summary>
/// Nature d'une anomalie de conformité légale détectée sur un planning.
/// </summary>
public enum TypeAnomalieConformite
{
    /// <summary>Le repos entre deux créneaux consécutifs est inférieur au minimum requis.</summary>
    ReposQuotidienInsuffisant,

    /// <summary>Aucun repos hebdomadaire d'une durée suffisante n'a été trouvé sur la semaine.</summary>
    ReposHebdomadaireInsuffisant,

    /// <summary>La durée de travail cumulée sur une journée dépasse le maximum autorisé.</summary>
    DureeQuotidienneDepassee,

    /// <summary>La durée de travail cumulée sur la semaine dépasse le maximum autorisé.</summary>
    DureeHebdomadaireDepassee,

    /// <summary>Une période de travail continue dépasse le seuil sans pause obligatoire.</summary>
    PauseObligatoireManquante,

    /// <summary>Le nombre de jours consécutifs travaillés dépasse le maximum autorisé.</summary>
    JoursConsecutifsDepasses
}
