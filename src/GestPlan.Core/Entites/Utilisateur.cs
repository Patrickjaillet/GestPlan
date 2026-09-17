using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Entites;

/// <summary>
/// Compte permettant de se connecter à l'application. L'authentification
/// (hachage des mots de passe, politique de sécurité) est mise en place en Phase 3.
/// </summary>
public class Utilisateur : EntiteBase
{
    public required string NomUtilisateur { get; set; }

    public required string HashMotDePasse { get; set; }

    public RoleUtilisateur Role { get; set; } = RoleUtilisateur.Consultation;

    public bool EstActif { get; set; } = true;

    public int? SiteAssigneId { get; set; }

    public Site? SiteAssigne { get; set; }
}
