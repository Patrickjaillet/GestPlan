namespace GestPlan.App.Services;

/// <summary>
/// Fournit les chaînes affichées à l'utilisateur à partir des ressources i18n.
/// Le mécanisme complet de sélection de langue est mis en place en Phase 10 ;
/// à ce stade, seul le français (<c>fr-FR.json</c>) est chargé.
/// </summary>
public interface IServiceLocalisation
{
    string ObtenirChaine(string cle);
}
