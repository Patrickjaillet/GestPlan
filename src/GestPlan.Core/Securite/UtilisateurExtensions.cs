using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Securite;

/// <summary>
/// Règles de contrôle d'accès applicables à un <see cref="Utilisateur"/> : rôle minimum
/// requis et cloisonnement des données par site.
/// </summary>
public static class UtilisateurExtensions
{
    /// <summary>
    /// Indique si l'utilisateur dispose au moins du rôle demandé. L'ordre des valeurs de
    /// <see cref="RoleUtilisateur"/> va du plus privilégié au moins privilégié.
    /// </summary>
    public static bool APourRoleMinimum(this Utilisateur utilisateur, RoleUtilisateur roleMinimum) =>
        utilisateur.Role <= roleMinimum;

    /// <summary>
    /// Indique si l'utilisateur peut accéder aux données du site donné :
    /// <see cref="RoleUtilisateur.Admin"/> accède à tous les sites, les autres rôles
    /// sont limités à leur site assigné (<see cref="Utilisateur.SiteAssigneId"/>).
    /// </summary>
    public static bool PeutAccederAuSite(this Utilisateur utilisateur, int siteId) =>
        utilisateur.Role == RoleUtilisateur.Admin || utilisateur.SiteAssigneId == siteId;
}
