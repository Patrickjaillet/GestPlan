using GestPlan.Core.Entites;

namespace GestPlan.Core.Planning;

/// <summary>
/// Règles de cohérence et de verrouillage applicables à un <see cref="CreneauPlanning"/>,
/// indépendamment de la détection de conflits avec d'autres créneaux.
/// </summary>
public static class ValidateurCreneau
{
    public static IReadOnlyList<string> Valider(CreneauPlanning creneau)
    {
        var erreurs = new List<string>();

        if (creneau.HeureFin <= creneau.HeureDebut)
        {
            erreurs.Add("L'heure de fin doit être postérieure à l'heure de début.");
        }

        return erreurs;
    }

    /// <summary>
    /// Un créneau dont la date est strictement antérieure à aujourd'hui est verrouillé :
    /// il ne peut plus être modifié sans un droit spécifique (rôle Admin).
    /// </summary>
    public static bool EstVerrouille(CreneauPlanning creneau, DateOnly aujourdHui) =>
        creneau.Date < aujourdHui;
}
