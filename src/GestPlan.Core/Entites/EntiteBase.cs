namespace GestPlan.Core.Entites;

/// <summary>
/// Base commune à toutes les entités persistées : identifiant et horodatage de suivi.
/// </summary>
public abstract class EntiteBase
{
    public int Id { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime? DateModification { get; set; }
}
