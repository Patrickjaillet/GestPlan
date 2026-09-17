namespace GestPlan.Core.Entites;

/// <summary>
/// Rattachement secondaire d'un employé à un site autre que son site principal
/// (<see cref="Employe.SitePrincipal"/>).
/// </summary>
public class EmployeSite
{
    public int EmployeId { get; set; }

    public Employe Employe { get; set; } = null!;

    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;
}
