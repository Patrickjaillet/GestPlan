namespace GestPlan.Core.Entites;

/// <summary>
/// Une indisponibilité récurrente d'un employé (ex : indisponible le lundi matin),
/// exprimée par un jour de la semaine et une plage horaire.
/// </summary>
public class Indisponibilite : EntiteBase
{
    public int EmployeId { get; set; }

    public Employe Employe { get; set; } = null!;

    public DayOfWeek JourSemaine { get; set; }

    public TimeOnly HeureDebut { get; set; }

    public TimeOnly HeureFin { get; set; }

    public string? Motif { get; set; }
}
