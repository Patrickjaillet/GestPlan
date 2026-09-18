using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Entites;

/// <summary>
/// Un créneau de travail planifié pour un employé, sur un site et un poste donnés.
/// </summary>
public class CreneauPlanning : EntiteBase
{
    public int EmployeId { get; set; }

    public Employe Employe { get; set; } = null!;

    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;

    public int PosteId { get; set; }

    public Poste Poste { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly HeureDebut { get; set; }

    public TimeOnly HeureFin { get; set; }

    public StatutCreneau Statut { get; set; } = StatutCreneau.Brouillon;

    /// <summary>
    /// Un créneau publié est visible par les employés et inclus dans les exports ;
    /// un créneau non publié (brouillon de travail) ne l'est pas, indépendamment
    /// de son <see cref="Statut"/>.
    /// </summary>
    public bool EstPublie { get; set; }

    public DateTime DebutHorodate => Date.ToDateTime(HeureDebut);

    public DateTime FinHorodate => Date.ToDateTime(HeureFin);

    /// <summary>
    /// Indique si ce créneau chevauche temporellement un autre créneau du même employé,
    /// le même jour. Ne tient pas compte du site/poste : un employé ne peut pas être
    /// affecté à deux créneaux simultanés, même sur des sites différents.
    /// </summary>
    public bool Chevauche(CreneauPlanning autre) =>
        EmployeId == autre.EmployeId &&
        Date == autre.Date &&
        Id != autre.Id &&
        HeureDebut < autre.HeureFin &&
        autre.HeureDebut < HeureFin;
}
