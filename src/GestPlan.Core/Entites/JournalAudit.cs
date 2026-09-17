namespace GestPlan.Core.Entites;

/// <summary>
/// Action réalisée sur une entité, pour la piste d'audit.
/// </summary>
public enum TypeActionAudit
{
    Creation,
    Modification,
    Suppression
}

/// <summary>
/// Trace d'audit d'une modification apportée aux données : qui, quoi, quand, avant/après.
/// Alimenté automatiquement par <see cref="GestPlan.Data.GestPlanDbContext.SaveChangesAsync"/>.
/// </summary>
public class JournalAudit
{
    public int Id { get; set; }

    public DateTime DateAction { get; set; }

    public int? UtilisateurId { get; set; }

    public required string NomEntite { get; set; }

    public required string CleEntite { get; set; }

    public TypeActionAudit Action { get; set; }

    public string? ValeursAvant { get; set; }

    public string? ValeursApres { get; set; }
}
