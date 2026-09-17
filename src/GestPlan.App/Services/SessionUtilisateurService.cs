using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;
using GestPlan.Core.Securite;

namespace GestPlan.App.Services;

/// <inheritdoc cref="ISessionUtilisateurService"/>
public class SessionUtilisateurService : ISessionUtilisateurService
{
    public Utilisateur? UtilisateurConnecte { get; private set; }

    public bool EstConnecte => UtilisateurConnecte is not null;

    public event EventHandler? SessionOuverte;

    public event EventHandler<Utilisateur>? SessionFermee;

    public void OuvrirSession(Utilisateur utilisateur)
    {
        UtilisateurConnecte = utilisateur;
        SessionOuverte?.Invoke(this, EventArgs.Empty);
    }

    public void FermerSession()
    {
        var utilisateurDeconnecte = UtilisateurConnecte;
        UtilisateurConnecte = null;

        if (utilisateurDeconnecte is not null)
        {
            SessionFermee?.Invoke(this, utilisateurDeconnecte);
        }
    }

    public bool APourRoleMinimum(RoleUtilisateur roleMinimum) =>
        UtilisateurConnecte?.APourRoleMinimum(roleMinimum) ?? false;

    public bool PeutAccederAuSite(int siteId) =>
        UtilisateurConnecte?.PeutAccederAuSite(siteId) ?? false;
}
