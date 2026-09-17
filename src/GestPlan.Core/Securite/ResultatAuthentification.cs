using GestPlan.Core.Entites;

namespace GestPlan.Core.Securite;

/// <summary>
/// Résultat d'une tentative d'authentification.
/// </summary>
public enum StatutAuthentification
{
    Succes,
    IdentifiantsInvalides,
    CompteInactif,
    CompteVerrouille,
    MotDePasseExpire
}

public record ResultatAuthentification(StatutAuthentification Statut, Utilisateur? Utilisateur)
{
    public static ResultatAuthentification Reussi(Utilisateur utilisateur) =>
        new(StatutAuthentification.Succes, utilisateur);

    public static ResultatAuthentification Echoue(StatutAuthentification statut) =>
        new(statut, null);
}
