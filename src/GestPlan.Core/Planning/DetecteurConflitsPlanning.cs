using GestPlan.Core.Entites;

namespace GestPlan.Core.Planning;

/// <summary>
/// Détecte les conflits de planning : double affectation (chevauchement de créneaux
/// pour un même employé) et affectation sur une période d'indisponibilité déclarée.
/// </summary>
public static class DetecteurConflitsPlanning
{
    /// <summary>
    /// Détecte les conflits du <paramref name="creneau"/> par rapport aux autres créneaux
    /// existants (<paramref name="autresCreneaux"/>, qui ne doit pas inclure <paramref name="creneau"/>
    /// lui-même) et aux indisponibilités déclarées de l'employé.
    /// </summary>
    public static IReadOnlyList<ConflitPlanning> Detecter(
        CreneauPlanning creneau,
        IEnumerable<CreneauPlanning> autresCreneaux,
        IEnumerable<Indisponibilite> indisponibilites)
    {
        var conflits = new List<ConflitPlanning>();

        foreach (var autre in autresCreneaux)
        {
            if (creneau.Chevauche(autre))
            {
                conflits.Add(new ConflitPlanning(TypeConflitPlanning.DoubleAffectation, creneau, autre));
            }
        }

        foreach (var indisponibilite in indisponibilites.Where(i => i.EmployeId == creneau.EmployeId))
        {
            if (indisponibilite.JourSemaine != creneau.Date.DayOfWeek)
            {
                continue;
            }

            if (creneau.HeureDebut < indisponibilite.HeureFin && indisponibilite.HeureDebut < creneau.HeureFin)
            {
                conflits.Add(new ConflitPlanning(TypeConflitPlanning.Indisponibilite, creneau, null));
            }
        }

        return conflits;
    }
}
