using GestPlan.Core.Entites;

namespace GestPlan.Core.Conformite;

/// <summary>
/// Moteur de règles de conformité légale : évalue les créneaux planifiés d'un employé
/// au regard des paramètres du <see cref="ReglesConformite"/> de son site et produit
/// la liste des anomalies détectées (repos, durées maximales, pauses, jours consécutifs).
/// </summary>
public static class MoteurConformite
{
    /// <summary>
    /// Évalue l'ensemble des <paramref name="creneaux"/> d'un même employé (peuvent
    /// couvrir plusieurs semaines) par rapport aux <paramref name="regles"/> fournies.
    /// </summary>
    public static IReadOnlyList<AnomalieConformite> Evaluer(int employeId, IEnumerable<CreneauPlanning> creneaux, ReglesConformite regles)
    {
        var creneauxTries = creneaux
            .Where(c => c.EmployeId == employeId)
            .OrderBy(c => c.Date)
            .ThenBy(c => c.HeureDebut)
            .ToList();

        var anomalies = new List<AnomalieConformite>();

        anomalies.AddRange(DetecterReposQuotidienInsuffisant(employeId, creneauxTries, regles));
        anomalies.AddRange(DetecterDureeQuotidienneDepassee(employeId, creneauxTries, regles));
        anomalies.AddRange(DetecterPauseObligatoireManquante(employeId, creneauxTries, regles));
        anomalies.AddRange(DetecterReposHebdomadaireInsuffisant(employeId, creneauxTries, regles));
        anomalies.AddRange(DetecterDureeHebdomadaireDepassee(employeId, creneauxTries, regles));
        anomalies.AddRange(DetecterJoursConsecutifsDepasses(employeId, creneauxTries, regles));

        return anomalies;
    }

    private static IEnumerable<AnomalieConformite> DetecterReposQuotidienInsuffisant(
        int employeId, IReadOnlyList<CreneauPlanning> creneaux, ReglesConformite regles)
    {
        for (var i = 0; i < creneaux.Count - 1; i++)
        {
            var actuel = creneaux[i];
            var suivant = creneaux[i + 1];

            var repos = suivant.DebutHorodate - actuel.FinHorodate;
            if (repos < regles.ReposQuotidienMinimum)
            {
                yield return new AnomalieConformite(TypeAnomalieConformite.ReposQuotidienInsuffisant, employeId, actuel.Date, actuel, suivant);
            }
        }
    }

    private static IEnumerable<AnomalieConformite> DetecterDureeQuotidienneDepassee(
        int employeId, IReadOnlyList<CreneauPlanning> creneaux, ReglesConformite regles)
    {
        foreach (var groupe in creneaux.GroupBy(c => c.Date))
        {
            var dureeTotale = groupe.Aggregate(TimeSpan.Zero, (cumul, c) => cumul + (c.HeureFin - c.HeureDebut));
            if (dureeTotale > regles.DureeMaximaleQuotidienne)
            {
                yield return new AnomalieConformite(TypeAnomalieConformite.DureeQuotidienneDepassee, employeId, groupe.Key, groupe.First(), null);
            }
        }
    }

    private static IEnumerable<AnomalieConformite> DetecterPauseObligatoireManquante(
        int employeId, IReadOnlyList<CreneauPlanning> creneaux, ReglesConformite regles)
    {
        foreach (var groupe in creneaux.GroupBy(c => c.Date))
        {
            var creneauxJour = groupe.OrderBy(c => c.HeureDebut).ToList();
            var dureeContinue = TimeSpan.Zero;

            for (var i = 0; i < creneauxJour.Count; i++)
            {
                dureeContinue += (creneauxJour[i].HeureFin - creneauxJour[i].HeureDebut);

                var pauseSuivante = i < creneauxJour.Count - 1
                    ? (creneauxJour[i + 1].HeureDebut - creneauxJour[i].HeureFin)
                    : TimeSpan.Zero;

                if (dureeContinue >= regles.DureeTravailContinuAvantPause &&
                    pauseSuivante < regles.DureePauseObligatoire)
                {
                    yield return new AnomalieConformite(TypeAnomalieConformite.PauseObligatoireManquante, employeId, groupe.Key, creneauxJour[i], null);
                    break;
                }

                if (pauseSuivante >= regles.DureePauseObligatoire)
                {
                    dureeContinue = TimeSpan.Zero;
                }
            }
        }
    }

    private static IEnumerable<AnomalieConformite> DetecterReposHebdomadaireInsuffisant(
        int employeId, IReadOnlyList<CreneauPlanning> creneaux, ReglesConformite regles)
    {
        foreach (var groupe in creneaux.GroupBy(c => NumeroDeSemaineIso(c.Date)))
        {
            var creneauxSemaine = groupe.OrderBy(c => c.Date).ThenBy(c => c.HeureDebut).ToList();

            var meilleurRepos = TrouverPlusLongRepos(creneauxSemaine);
            if (meilleurRepos < regles.ReposHebdomadaireMinimum)
            {
                yield return new AnomalieConformite(TypeAnomalieConformite.ReposHebdomadaireInsuffisant, employeId, creneauxSemaine[0].Date, creneauxSemaine[0], null);
            }
        }
    }

    private static TimeSpan TrouverPlusLongRepos(IReadOnlyList<CreneauPlanning> creneauxSemaine)
    {
        if (creneauxSemaine.Count == 0)
        {
            return TimeSpan.Zero;
        }

        var lundi = creneauxSemaine[0].Date.AddDays(-((int)creneauxSemaine[0].Date.DayOfWeek == 0 ? 6 : (int)creneauxSemaine[0].Date.DayOfWeek - 1));
        var debutSemaine = lundi.ToDateTime(TimeOnly.MinValue);
        var finSemaine = lundi.AddDays(7).ToDateTime(TimeOnly.MinValue);

        var plusLongRepos = TimeSpan.Zero;
        var curseur = debutSemaine;

        foreach (var creneau in creneauxSemaine)
        {
            var repos = creneau.DebutHorodate - curseur;
            if (repos > plusLongRepos)
            {
                plusLongRepos = repos;
            }

            if (creneau.FinHorodate > curseur)
            {
                curseur = creneau.FinHorodate;
            }
        }

        var reposFinal = finSemaine - curseur;
        if (reposFinal > plusLongRepos)
        {
            plusLongRepos = reposFinal;
        }

        return plusLongRepos;
    }

    private static IEnumerable<AnomalieConformite> DetecterDureeHebdomadaireDepassee(
        int employeId, IReadOnlyList<CreneauPlanning> creneaux, ReglesConformite regles)
    {
        foreach (var groupe in creneaux.GroupBy(c => NumeroDeSemaineIso(c.Date)))
        {
            var dureeTotale = groupe.Aggregate(TimeSpan.Zero, (cumul, c) => cumul + (c.HeureFin - c.HeureDebut));
            if (dureeTotale > regles.DureeMaximaleHebdomadaire)
            {
                var premier = groupe.OrderBy(c => c.Date).ThenBy(c => c.HeureDebut).First();
                yield return new AnomalieConformite(TypeAnomalieConformite.DureeHebdomadaireDepassee, employeId, premier.Date, premier, null);
            }
        }
    }

    private static IEnumerable<AnomalieConformite> DetecterJoursConsecutifsDepasses(
        int employeId, IReadOnlyList<CreneauPlanning> creneaux, ReglesConformite regles)
    {
        var joursTravailles = creneaux.Select(c => c.Date).Distinct().OrderBy(d => d).ToList();

        var debutSerie = 0;
        for (var i = 1; i <= joursTravailles.Count; i++)
        {
            var seriesRompue = i == joursTravailles.Count || joursTravailles[i] != joursTravailles[i - 1].AddDays(1);

            if (seriesRompue)
            {
                var longueurSerie = i - debutSerie;
                if (longueurSerie > regles.JoursConsecutifsMaximum)
                {
                    var jourDeDepassement = joursTravailles[debutSerie + regles.JoursConsecutifsMaximum];
                    var creneauDuJour = creneaux.First(c => c.Date == jourDeDepassement);
                    yield return new AnomalieConformite(TypeAnomalieConformite.JoursConsecutifsDepasses, employeId, jourDeDepassement, creneauDuJour, null);
                }

                debutSerie = i;
            }
        }
    }

    private static int NumeroDeSemaineIso(DateOnly date)
    {
        var lundi = date.AddDays(-((int)date.DayOfWeek == 0 ? 6 : (int)date.DayOfWeek - 1));
        return lundi.DayNumber / 7;
    }
}
