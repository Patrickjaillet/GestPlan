using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.App.Services;
using GestPlan.Core.Conformite;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;
using GestPlan.Core.Planning;
using GestPlan.Data.Repositories;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Écran de planning visuel : grille par semaine/mois/employé/poste, création et
/// modification de créneaux, détection de conflits, undo/redo, mode brouillon/publié.
/// </summary>
public partial class PlanningViewModel : PageViewModelBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ISelecteurSiteService _selecteurSite;
    private readonly ISessionUtilisateurService _session;
    private readonly IServicePileAnnulation _pileAnnulation;

    [ObservableProperty]
    private TypeVuePlanning _vueSelectionnee = TypeVuePlanning.Semaine;

    [ObservableProperty]
    private DateOnly _dateReference = DateOnly.FromDateTime(DateTime.Today);

    [ObservableProperty]
    private string? _messageErreur;

    /// <summary>
    /// Vue consolidée multi-site : activée lorsque le sélecteur de site global est
    /// positionné sur « tous les sites » (<see cref="ISelecteurSiteService.SiteSelectionneId"/> nul),
    /// affiche les créneaux groupés visuellement par site pour permettre leur comparaison.
    /// </summary>
    [ObservableProperty]
    private bool _afficherVueConsolidee;

    public ObservableCollection<DateOnly> JoursAffiches { get; } = [];

    public ObservableCollection<Employe> Employes { get; } = [];

    public ObservableCollection<Poste> Postes { get; } = [];

    /// <summary>
    /// Colonnes de la grille pour les vues <see cref="TypeVuePlanning.ParEmploye"/> et
    /// <see cref="TypeVuePlanning.ParPoste"/> (l'employé ou le poste remplace le jour en colonne,
    /// une seule journée — <see cref="DateReference"/> — étant affichée dans ce mode).
    /// </summary>
    public ObservableCollection<ColonneEntitePlanning> ColonnesParEntite { get; } = [];

    public ObservableCollection<CreneauPlanningViewModel> Creneaux { get; } = [];

    public bool PeutAnnuler => _pileAnnulation.PeutAnnuler;

    public bool PeutRetablir => _pileAnnulation.PeutRetablir;

    public PlanningViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        ISelecteurSiteService selecteurSite,
        ISessionUtilisateurService session,
        IServicePileAnnulation pileAnnulation)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _selecteurSite = selecteurSite;
        _session = session;
        _pileAnnulation = pileAnnulation;

        _pileAnnulation.Change += (_, _) =>
        {
            OnPropertyChanged(nameof(PeutAnnuler));
            OnPropertyChanged(nameof(PeutRetablir));
        };

        _selecteurSite.SiteChange += async (_, _) => await ChargerAsync();

        _ = ChargerAsync();
    }

    partial void OnVueSelectionneeChanged(TypeVuePlanning value) => _ = ChargerAsync();

    partial void OnDateReferenceChanged(DateOnly value) => _ = ChargerAsync();

    [RelayCommand]
    private void SemaineSuivante() => DateReference = DateReference.AddDays(7);

    [RelayCommand]
    private void SemainePrecedente() => DateReference = DateReference.AddDays(-7);

    [RelayCommand]
    private void AujourdHui() => DateReference = DateOnly.FromDateTime(DateTime.Today);

    private async Task ChargerAsync()
    {
        CalculerJoursAffiches();

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var siteId = _selecteurSite.SiteSelectionneId;

        var employes = await unitOfWork.Employes.ObtenirTousAsync();
        Employes.Clear();
        foreach (var employe in employes
                     .Where(e => !e.EstArchive && (siteId is null || e.SitePrincipalId == siteId))
                     .OrderBy(e => e.Nom).ThenBy(e => e.Prenom))
        {
            Employes.Add(employe);
        }

        var postes = await unitOfWork.Postes.ObtenirTousAsync();
        Postes.Clear();
        foreach (var poste in postes.Where(p => siteId is null || p.SiteId == siteId).OrderBy(p => p.Nom))
        {
            Postes.Add(poste);
        }

        ColonnesParEntite.Clear();
        if (VueSelectionnee == TypeVuePlanning.ParEmploye)
        {
            foreach (var employe in Employes)
            {
                ColonnesParEntite.Add(new ColonneEntitePlanning(employe.Id, $"{employe.Nom} {employe.Prenom}"));
            }
        }
        else if (VueSelectionnee == TypeVuePlanning.ParPoste)
        {
            foreach (var poste in Postes)
            {
                ColonnesParEntite.Add(new ColonneEntitePlanning(poste.Id, poste.Nom));
            }
        }

        var tousLesCreneaux = await unitOfWork.CreneauxPlanning.ObtenirTousAsync();
        var indisponibilites = await unitOfWork.Indisponibilites.ObtenirTousAsync();
        var toutesLesReglesConformite = await unitOfWork.ReglesConformite.ObtenirTousAsync();
        var reglesConformiteParSite = toutesLesReglesConformite.ToDictionary(r => r.SiteId, r => r);

        var premierJour = JoursAffiches.Count > 0 ? JoursAffiches[0] : DateReference;
        var dernierJour = JoursAffiches.Count > 0 ? JoursAffiches[^1] : DateReference;

        var creneauxPeriode = tousLesCreneaux
            .Where(c => c.Date >= premierJour && c.Date <= dernierJour)
            .Where(c => siteId is null || c.SiteId == siteId)
            .ToList();

        var employesParId = employes.ToDictionary(e => e.Id, e => $"{e.Nom} {e.Prenom}");
        var postesParId = postes.ToDictionary(p => p.Id, p => p);

        var tousLesSites = await unitOfWork.Sites.ObtenirTousAsync();
        var sitesParId = tousLesSites.ToDictionary(s => s.Id, s => s.Nom);

        var anomaliesParCreneauId = new Dictionary<int, List<AnomalieConformite>>();
        foreach (var groupeEmploye in tousLesCreneaux.GroupBy(c => c.EmployeId))
        {
            var siteEmploye = groupeEmploye.Select(c => c.SiteId).FirstOrDefault();
            if (!reglesConformiteParSite.TryGetValue(siteEmploye, out var regles))
            {
                continue;
            }

            foreach (var anomalie in MoteurConformite.Evaluer(groupeEmploye.Key, groupeEmploye, regles))
            {
                if (anomalie.Creneau is null)
                {
                    continue;
                }

                if (!anomaliesParCreneauId.TryGetValue(anomalie.Creneau.Id, out var liste))
                {
                    liste = [];
                    anomaliesParCreneauId[anomalie.Creneau.Id] = liste;
                }
                liste.Add(anomalie);
            }
        }

        Creneaux.Clear();
        var aujourdHui = DateOnly.FromDateTime(DateTime.Today);

        foreach (var creneau in creneauxPeriode.OrderBy(c => c.Date).ThenBy(c => c.HeureDebut))
        {
            var poste = postesParId.GetValueOrDefault(creneau.PosteId);
            var nomEmploye = employesParId.GetValueOrDefault(creneau.EmployeId, string.Empty);
            var nomSite = sitesParId.GetValueOrDefault(creneau.SiteId, string.Empty);

            var vm = CreneauPlanningViewModel.DepuisEntite(creneau, nomEmploye, nomSite, poste?.Nom ?? string.Empty, poste?.CouleurAffichage);
            vm.EstVerrouille = ValidateurCreneau.EstVerrouille(creneau, aujourdHui) && !_session.APourRoleMinimum(RoleUtilisateur.Admin);

            var autresCreneaux = tousLesCreneaux.Where(c => c.Id != creneau.Id);
            vm.EnConflit = DetecteurConflitsPlanning.Detecter(creneau, autresCreneaux, indisponibilites).Count > 0;

            if (anomaliesParCreneauId.TryGetValue(creneau.Id, out var anomalies))
            {
                vm.EnAnomalieConformite = true;
                vm.MessageAnomaliesConformite = string.Join(Environment.NewLine, anomalies.Select(a => LibellesAnomalieConformite.Obtenir(a.Type)).Distinct());
            }

            Creneaux.Add(vm);
        }

        AfficherVueConsolidee = siteId is null;
    }

    private void CalculerJoursAffiches()
    {
        JoursAffiches.Clear();

        switch (VueSelectionnee)
        {
            case TypeVuePlanning.Semaine:
                var lundi = DateReference.AddDays(-((int)DateReference.DayOfWeek == 0 ? 6 : (int)DateReference.DayOfWeek - 1));
                for (var i = 0; i < 7; i++)
                {
                    JoursAffiches.Add(lundi.AddDays(i));
                }
                break;

            case TypeVuePlanning.ParEmploye:
            case TypeVuePlanning.ParPoste:
                // Ces vues affichent une seule journée, avec les employés/postes en colonnes
                // (au lieu des jours) : voir ColonnesParEntite.
                JoursAffiches.Add(DateReference);
                break;

            case TypeVuePlanning.Mois:
                var premierDuMois = new DateOnly(DateReference.Year, DateReference.Month, 1);
                var nombreDeJours = DateTime.DaysInMonth(DateReference.Year, DateReference.Month);
                for (var i = 0; i < nombreDeJours; i++)
                {
                    JoursAffiches.Add(premierDuMois.AddDays(i));
                }
                break;
        }
    }

    [RelayCommand]
    private async Task CreerCreneauAsync(NouveauCreneauParametres parametres)
    {
        MessageErreur = null;

        var creneau = new CreneauPlanning
        {
            EmployeId = parametres.EmployeId,
            SiteId = _selecteurSite.SiteSelectionneId ?? parametres.SiteId,
            PosteId = parametres.PosteId,
            Date = parametres.Date,
            HeureDebut = parametres.HeureDebut,
            HeureFin = parametres.HeureFin,
            Statut = StatutCreneau.Brouillon
        };

        var erreurs = ValidateurCreneau.Valider(creneau);
        if (erreurs.Count > 0)
        {
            MessageErreur = string.Join(" ", erreurs);
            return;
        }

        using (var unitOfWork = _unitOfWorkFactory.Creer())
        {
            await unitOfWork.CreneauxPlanning.AjouterAsync(creneau);
            await unitOfWork.EnregistrerAsync();
        }

        _pileAnnulation.Empiler(
            annuler: () => SupprimerCreneauInterneAsync(creneau.Id),
            retablir: () => RecreerCreneauAsync(creneau));

        await ChargerAsync();
    }

    private async Task SupprimerCreneauInterneAsync(int creneauId)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.CreneauxPlanning.ObtenirParIdAsync(creneauId);
        if (entite is not null)
        {
            unitOfWork.CreneauxPlanning.Supprimer(entite);
            await unitOfWork.EnregistrerAsync();
        }

        await ChargerAsync();
    }

    private async Task RecreerCreneauAsync(CreneauPlanning creneau)
    {
        using (var unitOfWork = _unitOfWorkFactory.Creer())
        {
            var copie = new CreneauPlanning
            {
                EmployeId = creneau.EmployeId,
                SiteId = creneau.SiteId,
                PosteId = creneau.PosteId,
                Date = creneau.Date,
                HeureDebut = creneau.HeureDebut,
                HeureFin = creneau.HeureFin,
                Statut = creneau.Statut,
                EstPublie = creneau.EstPublie
            };

            await unitOfWork.CreneauxPlanning.AjouterAsync(copie);
            await unitOfWork.EnregistrerAsync();

            creneau.Id = copie.Id;
        }

        await ChargerAsync();
    }

    [RelayCommand]
    private async Task RedimensionnerCreneauAsync(RedimensionnementParametres parametres)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var creneau = await unitOfWork.CreneauxPlanning.ObtenirParIdAsync(parametres.CreneauId);
        if (creneau is null)
        {
            return;
        }

        var ancienDebut = creneau.HeureDebut;
        var ancienneFin = creneau.HeureFin;

        creneau.HeureDebut = parametres.NouvelleHeureDebut;
        creneau.HeureFin = parametres.NouvelleHeureFin;

        var erreurs = ValidateurCreneau.Valider(creneau);
        if (erreurs.Count > 0)
        {
            MessageErreur = string.Join(" ", erreurs);
            return;
        }

        unitOfWork.CreneauxPlanning.Modifier(creneau);
        await unitOfWork.EnregistrerAsync();

        _pileAnnulation.Empiler(
            annuler: () => RestaurerHorairesAsync(parametres.CreneauId, ancienDebut, ancienneFin),
            retablir: () => RestaurerHorairesAsync(parametres.CreneauId, parametres.NouvelleHeureDebut, parametres.NouvelleHeureFin));

        await ChargerAsync();
    }

    private async Task RestaurerHorairesAsync(int creneauId, TimeOnly debut, TimeOnly fin)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var creneau = await unitOfWork.CreneauxPlanning.ObtenirParIdAsync(creneauId);
        if (creneau is not null)
        {
            creneau.HeureDebut = debut;
            creneau.HeureFin = fin;
            unitOfWork.CreneauxPlanning.Modifier(creneau);
            await unitOfWork.EnregistrerAsync();
        }

        await ChargerAsync();
    }

    [RelayCommand]
    private async Task SupprimerCreneauAsync(CreneauPlanningViewModel creneau)
    {
        if (creneau.EstVerrouille)
        {
            MessageErreur = "Ce créneau est verrouillé (date passée) et ne peut être supprimé.";
            return;
        }

        CreneauPlanning? entiteSupprimee;
        using (var unitOfWork = _unitOfWorkFactory.Creer())
        {
            entiteSupprimee = await unitOfWork.CreneauxPlanning.ObtenirParIdAsync(creneau.Id);
            if (entiteSupprimee is null)
            {
                return;
            }

            unitOfWork.CreneauxPlanning.Supprimer(entiteSupprimee);
            await unitOfWork.EnregistrerAsync();
        }

        var copie = entiteSupprimee;
        _pileAnnulation.Empiler(
            annuler: () => RecreerCreneauAsync(copie),
            retablir: () => SupprimerCreneauInterneAsync(copie.Id));

        await ChargerAsync();
    }

    [RelayCommand]
    private async Task ChangerStatutAsync((CreneauPlanningViewModel Creneau, StatutCreneau NouveauStatut) parametres)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.CreneauxPlanning.ObtenirParIdAsync(parametres.Creneau.Id);
        if (entite is null)
        {
            return;
        }

        entite.Statut = parametres.NouveauStatut;
        unitOfWork.CreneauxPlanning.Modifier(entite);
        await unitOfWork.EnregistrerAsync();

        await ChargerAsync();
    }

    [RelayCommand]
    private async Task PublierSemaineAsync()
    {
        MessageErreur = null;

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var tousLesCreneaux = await unitOfWork.CreneauxPlanning.ObtenirTousAsync();
        var premierJour = JoursAffiches.Count > 0 ? JoursAffiches[0] : DateReference;
        var dernierJour = JoursAffiches.Count > 0 ? JoursAffiches[^1] : DateReference;

        var creneauxAPublier = tousLesCreneaux.Where(c => c.Date >= premierJour && c.Date <= dernierJour).ToList();

        var toutesLesReglesConformite = await unitOfWork.ReglesConformite.ObtenirTousAsync();
        var reglesConformiteParSite = toutesLesReglesConformite.ToDictionary(r => r.SiteId, r => r);

        foreach (var groupeEmploye in creneauxAPublier.GroupBy(c => c.EmployeId))
        {
            var siteEmploye = groupeEmploye.Select(c => c.SiteId).FirstOrDefault();
            if (!reglesConformiteParSite.TryGetValue(siteEmploye, out var regles) || !regles.BloquerPublicationSiNonConforme)
            {
                continue;
            }

            var creneauxCompletEmploye = tousLesCreneaux.Where(c => c.EmployeId == groupeEmploye.Key);
            if (MoteurConformite.Evaluer(groupeEmploye.Key, creneauxCompletEmploye, regles).Count > 0)
            {
                MessageErreur = "La publication a été bloquée : au moins une anomalie de conformité n'est pas résolue sur cette période. Corrigez les créneaux signalés ou désactivez le blocage dans les règles de conformité du site.";
                return;
            }
        }

        foreach (var creneau in creneauxAPublier)
        {
            creneau.EstPublie = true;
            if (creneau.Statut == StatutCreneau.Brouillon)
            {
                creneau.Statut = StatutCreneau.Planifie;
            }

            unitOfWork.CreneauxPlanning.Modifier(creneau);
        }

        await unitOfWork.EnregistrerAsync();
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task DupliquerSemaineAsync(int nombreDeSemaines)
    {
        if (nombreDeSemaines <= 0)
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var tousLesCreneaux = await unitOfWork.CreneauxPlanning.ObtenirTousAsync();
        var premierJour = JoursAffiches.Count > 0 ? JoursAffiches[0] : DateReference;
        var dernierJour = JoursAffiches.Count > 0 ? JoursAffiches[^1] : DateReference;

        var creneauxSemaine = tousLesCreneaux
            .Where(c => c.Date >= premierJour && c.Date <= dernierJour)
            .ToList();

        for (var semaine = 1; semaine <= nombreDeSemaines; semaine++)
        {
            var decalage = 7 * semaine;

            foreach (var source in creneauxSemaine)
            {
                await unitOfWork.CreneauxPlanning.AjouterAsync(new CreneauPlanning
                {
                    EmployeId = source.EmployeId,
                    SiteId = source.SiteId,
                    PosteId = source.PosteId,
                    Date = source.Date.AddDays(decalage),
                    HeureDebut = source.HeureDebut,
                    HeureFin = source.HeureFin,
                    Statut = StatutCreneau.Brouillon,
                    EstPublie = false
                });
            }
        }

        await unitOfWork.EnregistrerAsync();
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task AnnulerAsync() => await _pileAnnulation.AnnulerAsync();

    [RelayCommand]
    private async Task RetablirAsync() => await _pileAnnulation.RetablirAsync();
}

public record NouveauCreneauParametres(int EmployeId, int SiteId, int PosteId, DateOnly Date, TimeOnly HeureDebut, TimeOnly HeureFin);

public record RedimensionnementParametres(int CreneauId, TimeOnly NouvelleHeureDebut, TimeOnly NouvelleHeureFin);

/// <summary>Une colonne de la grille en vue « Par employé » ou « Par poste » : l'identifiant
/// sert à filtrer les créneaux de cette colonne (EmployeId ou PosteId selon la vue active).</summary>
public record ColonneEntitePlanning(int Id, string Libelle);
