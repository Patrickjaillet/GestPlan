using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.Core.Conges;

/// <summary>
/// Règles de cohérence applicables à une demande d'absence : validité des dates,
/// non-chevauchement avec d'autres absences ou avec des créneaux de planning existants.
/// </summary>
public static class ValidateurAbsence
{
    public static IReadOnlyList<string> Valider(Absence absence)
    {
        var erreurs = new List<string>();

        if (absence.DateFin < absence.DateDebut)
        {
            erreurs.Add("La date de fin doit être postérieure ou égale à la date de début.");
        }

        return erreurs;
    }

    /// <summary>
    /// Indique si l'<paramref name="absence"/> chevauche une autre absence déjà validée ou demandée
    /// du même employé (hors elle-même).
    /// </summary>
    public static bool ChevaucheUneAutreAbsence(Absence absence, IEnumerable<Absence> autresAbsences) =>
        autresAbsences.Any(autre =>
            autre.Id != absence.Id &&
            autre.EmployeId == absence.EmployeId &&
            autre.Statut != StatutAbsence.Refusee &&
            absence.DateDebut <= autre.DateFin &&
            autre.DateDebut <= absence.DateFin);

    /// <summary>
    /// Indique si l'<paramref name="absence"/> chevauche un créneau de planning déjà existant
    /// pour le même employé — utilisé pour bloquer la planification sur une absence validée et
    /// pour alerter en cas de tentative de chevauchement.
    /// </summary>
    public static bool ChevaucheUnCreneauPlanning(Absence absence, IEnumerable<CreneauPlanning> creneaux) =>
        creneaux.Any(creneau =>
            creneau.EmployeId == absence.EmployeId &&
            absence.DateDebut <= creneau.Date &&
            creneau.Date <= absence.DateFin);
}
