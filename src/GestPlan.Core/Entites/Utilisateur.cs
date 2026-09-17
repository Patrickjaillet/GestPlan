using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Entites;

/// <summary>
/// Compte permettant de se connecter à l'application.
/// </summary>
public class Utilisateur : EntiteBase
{
    public required string NomUtilisateur { get; set; }

    public required string HashMotDePasse { get; set; }

    public DateTime DateDernierChangementMotDePasse { get; set; } = DateTime.UtcNow;

    public RoleUtilisateur Role { get; set; } = RoleUtilisateur.Consultation;

    public bool EstActif { get; set; } = true;

    public DateTime? DerniereConnexion { get; set; }

    public int TentativesEchoueesConsecutives { get; set; }

    public DateTime? VerrouilleJusqua { get; set; }

    public int? SiteAssigneId { get; set; }

    public Site? SiteAssigne { get; set; }
}
