using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;
using GestPlan.Core.Securite;

namespace GestPlan.Tests;

public class UtilisateurExtensionsTests
{
    [Theory]
    [InlineData(RoleUtilisateur.Admin, RoleUtilisateur.Admin, true)]
    [InlineData(RoleUtilisateur.Admin, RoleUtilisateur.Manager, true)]
    [InlineData(RoleUtilisateur.Admin, RoleUtilisateur.Consultation, true)]
    [InlineData(RoleUtilisateur.Manager, RoleUtilisateur.Admin, false)]
    [InlineData(RoleUtilisateur.Manager, RoleUtilisateur.Manager, true)]
    [InlineData(RoleUtilisateur.Manager, RoleUtilisateur.Consultation, true)]
    [InlineData(RoleUtilisateur.Consultation, RoleUtilisateur.Admin, false)]
    [InlineData(RoleUtilisateur.Consultation, RoleUtilisateur.Manager, false)]
    [InlineData(RoleUtilisateur.Consultation, RoleUtilisateur.Consultation, true)]
    public void APourRoleMinimum_AucuneElevationDePrivilegesNEstPossible(
        RoleUtilisateur roleReel, RoleUtilisateur roleMinimumRequis, bool accesAttendu)
    {
        var utilisateur = new Utilisateur
        {
            NomUtilisateur = "test",
            HashMotDePasse = "hash",
            Role = roleReel
        };

        Assert.Equal(accesAttendu, utilisateur.APourRoleMinimum(roleMinimumRequis));
    }

    [Theory]
    [InlineData(RoleUtilisateur.Admin, 1, 1, true)]
    [InlineData(RoleUtilisateur.Admin, 1, 2, true)]
    [InlineData(RoleUtilisateur.Manager, 1, 1, true)]
    [InlineData(RoleUtilisateur.Manager, 1, 2, false)]
    [InlineData(RoleUtilisateur.Consultation, 1, 1, true)]
    [InlineData(RoleUtilisateur.Consultation, 1, 2, false)]
    public void PeutAccederAuSite_AdminAccedeATousLesSites_AutresRolesLimitesALeurSiteAssigne(
        RoleUtilisateur role, int siteAssigneId, int siteConsulte, bool accesAttendu)
    {
        var utilisateur = new Utilisateur
        {
            NomUtilisateur = "test",
            HashMotDePasse = "hash",
            Role = role,
            SiteAssigneId = siteAssigneId
        };

        Assert.Equal(accesAttendu, utilisateur.PeutAccederAuSite(siteConsulte));
    }

    [Fact]
    public void PeutAccederAuSite_UnManagerSansSiteAssigne_NAccedeAAucunSite()
    {
        var utilisateur = new Utilisateur
        {
            NomUtilisateur = "sans-site",
            HashMotDePasse = "hash",
            Role = RoleUtilisateur.Manager,
            SiteAssigneId = null
        };

        Assert.False(utilisateur.PeutAccederAuSite(1));
        Assert.False(utilisateur.PeutAccederAuSite(2));
    }
}
