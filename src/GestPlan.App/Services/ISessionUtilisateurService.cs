using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.App.Services;

/// <summary>
/// Détient l'utilisateur actuellement connecté à l'application et notifie
/// les composants concernés lors de la connexion ou de la déconnexion.
/// </summary>
public interface ISessionUtilisateurService
{
    Utilisateur? UtilisateurConnecte { get; }

    bool EstConnecte { get; }

    event EventHandler? SessionOuverte;

    event EventHandler<Utilisateur>? SessionFermee;

    void OuvrirSession(Utilisateur utilisateur);

    void FermerSession();

    /// <summary>
    /// Indique si l'utilisateur connecté dispose au moins du rôle demandé
    /// (<see cref="RoleUtilisateur.Admin"/> &gt; <see cref="RoleUtilisateur.Manager"/> &gt; <see cref="RoleUtilisateur.Consultation"/>).
    /// </summary>
    bool APourRoleMinimum(RoleUtilisateur roleMinimum);

    /// <summary>
    /// Indique si l'utilisateur connecté peut accéder aux données du site donné
    /// (Admin : tous sites ; Manager/Consultation : uniquement leur site assigné).
    /// </summary>
    bool PeutAccederAuSite(int siteId);
}
