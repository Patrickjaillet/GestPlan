using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;
using GestPlan.Data.Repositories;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Écran de gestion des employés : liste avec recherche/filtres, fiche détaillée
/// (identité, contrats, compétences, disponibilités), archivage.
/// </summary>
public partial class EmployesViewModel : PageViewModelBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    private List<Employe> _tousLesEmployes = [];

    [ObservableProperty]
    private string _texteRecherche = string.Empty;

    [ObservableProperty]
    private SiteFiltreItem? _siteSelectionne;

    [ObservableProperty]
    private PosteFiltreItem? _posteSelectionne;

    [ObservableProperty]
    private StatutFiltreItem _statutSelectionne = StatutFiltreItem.Tous;

    [ObservableProperty]
    private EmployeEditableViewModel? _employeSelectionne;

    [ObservableProperty]
    private string? _messageErreur;

    public ObservableCollection<Employe> Employes { get; } = [];

    public ObservableCollection<SiteFiltreItem> SitesDisponibles { get; } = [];

    public ObservableCollection<PosteFiltreItem> PostesDisponibles { get; } = [];

    public ObservableCollection<ContratEditableViewModel> Contrats { get; } = [];

    public ObservableCollection<PosteFiltreItem> CompetencesDisponibles { get; } = [];

    public ObservableCollection<IndisponibiliteEditableViewModel> Indisponibilites { get; } = [];

    public EmployesViewModel(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _ = ChargerAsync();
    }

    [RelayCommand]
    private async Task RechargerAsync() => await ChargerAsync();

    private async Task ChargerAsync()
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        _tousLesEmployes = (await unitOfWork.Employes.ObtenirTousAsync()).ToList();

        var sites = await unitOfWork.Sites.ObtenirTousAsync();
        SitesDisponibles.Clear();
        foreach (var site in sites.OrderBy(s => s.Nom))
        {
            SitesDisponibles.Add(new SiteFiltreItem(site.Id, site.Nom));
        }

        var postes = await unitOfWork.Postes.ObtenirTousAsync();
        PostesDisponibles.Clear();
        CompetencesDisponibles.Clear();
        foreach (var poste in postes.OrderBy(p => p.Nom))
        {
            PostesDisponibles.Add(new PosteFiltreItem(poste.Id, poste.Nom));
            CompetencesDisponibles.Add(new PosteFiltreItem(poste.Id, poste.Nom));
        }

        AppliquerFiltres();
    }

    partial void OnTexteRechercheChanged(string value) => AppliquerFiltres();

    partial void OnSiteSelectionneChanged(SiteFiltreItem? value) => AppliquerFiltres();

    partial void OnPosteSelectionneChanged(PosteFiltreItem? value) => AppliquerFiltres();

    partial void OnStatutSelectionneChanged(StatutFiltreItem value) => AppliquerFiltres();

    private void AppliquerFiltres()
    {
        var resultat = _tousLesEmployes.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(TexteRecherche))
        {
            var recherche = TexteRecherche.Trim();
            resultat = resultat.Where(e =>
                e.Nom.Contains(recherche, StringComparison.OrdinalIgnoreCase) ||
                e.Prenom.Contains(recherche, StringComparison.OrdinalIgnoreCase));
        }

        if (SiteSelectionne is not null)
        {
            resultat = resultat.Where(e => e.SitePrincipalId == SiteSelectionne.Id);
        }

        if (PosteSelectionne is not null)
        {
            resultat = resultat.Where(e => e.PostesAutorises.Any(pa => pa.PosteId == PosteSelectionne.Id));
        }

        resultat = StatutSelectionne switch
        {
            StatutFiltreItem.Actif => resultat.Where(e => !e.EstArchive),
            StatutFiltreItem.Archive => resultat.Where(e => e.EstArchive),
            _ => resultat
        };

        Employes.Clear();
        foreach (var employe in resultat.OrderBy(e => e.Nom).ThenBy(e => e.Prenom))
        {
            Employes.Add(employe);
        }
    }

    [RelayCommand]
    private void PreparerNouvelEmploye()
    {
        EmployeSelectionne = new EmployeEditableViewModel
        {
            SitePrincipalId = SitesDisponibles.FirstOrDefault()?.Id ?? 0
        };
        Contrats.Clear();
        Indisponibilites.Clear();
        CompetencesEmploye.Clear();
        MessageErreur = null;
    }

    [RelayCommand]
    private async Task SelectionnerEmployeAsync(Employe employe)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.Employes.ObtenirParIdAsync(employe.Id);
        if (entite is null)
        {
            return;
        }

        EmployeSelectionne = new EmployeEditableViewModel(entite);
        MessageErreur = null;

        await ChargerContratsAsync(employe.Id);
        await ChargerIndisponibilitesAsync(employe.Id);
        await ChargerCompetencesAsync(employe.Id);
        PreparerNouveauContrat();
    }

    private async Task ChargerContratsAsync(int employeId)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var contrats = await unitOfWork.Contrats.ObtenirTousAsync();
        var sites = await unitOfWork.Sites.ObtenirTousAsync();
        var postes = await unitOfWork.Postes.ObtenirTousAsync();

        Contrats.Clear();
        foreach (var contrat in contrats.Where(c => c.EmployeId == employeId).OrderByDescending(c => c.DateDebut))
        {
            contrat.Site ??= sites.First(s => s.Id == contrat.SiteId);
            contrat.Poste ??= postes.First(p => p.Id == contrat.PosteId);
            Contrats.Add(new ContratEditableViewModel(contrat));
        }
    }

    private async Task ChargerIndisponibilitesAsync(int employeId)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var indisponibilites = await unitOfWork.Indisponibilites.ObtenirTousAsync();

        Indisponibilites.Clear();
        foreach (var indisponibilite in indisponibilites
                     .Where(i => i.EmployeId == employeId)
                     .OrderBy(i => i.JourSemaine).ThenBy(i => i.HeureDebut))
        {
            Indisponibilites.Add(new IndisponibiliteEditableViewModel(indisponibilite));
        }
    }

    [RelayCommand]
    private async Task EnregistrerEmployeAsync()
    {
        MessageErreur = null;

        if (EmployeSelectionne is null || string.IsNullOrWhiteSpace(EmployeSelectionne.Nom) || string.IsNullOrWhiteSpace(EmployeSelectionne.Prenom))
        {
            MessageErreur = "Le nom et le prénom sont obligatoires.";
            return;
        }

        if (EmployeSelectionne.SitePrincipalId == 0)
        {
            MessageErreur = "Le site principal est obligatoire.";
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        if (EmployeSelectionne.Id == 0)
        {
            var nouvelEmploye = new Employe { Nom = EmployeSelectionne.Nom, Prenom = EmployeSelectionne.Prenom, SitePrincipalId = EmployeSelectionne.SitePrincipalId };
            EmployeSelectionne.AppliquerA(nouvelEmploye);
            await unitOfWork.Employes.AjouterAsync(nouvelEmploye);
        }
        else
        {
            var employe = await unitOfWork.Employes.ObtenirParIdAsync(EmployeSelectionne.Id);
            if (employe is null)
            {
                return;
            }

            EmployeSelectionne.AppliquerA(employe);
            unitOfWork.Employes.Modifier(employe);
        }

        await unitOfWork.EnregistrerAsync();
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task BasculerArchivageAsync(Employe employe)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.Employes.ObtenirParIdAsync(employe.Id);
        if (entite is null)
        {
            return;
        }

        entite.EstArchive = !entite.EstArchive;
        unitOfWork.Employes.Modifier(entite);
        await unitOfWork.EnregistrerAsync();
        await ChargerAsync();
    }

    [ObservableProperty]
    private ContratEditableViewModel _nouveauContrat = new();

    [RelayCommand]
    private void PreparerNouveauContrat()
    {
        NouveauContrat = new ContratEditableViewModel
        {
            SiteId = EmployeSelectionne?.SitePrincipalId ?? 0
        };
    }

    [RelayCommand]
    private async Task AjouterContratAsync()
    {
        if (EmployeSelectionne is null || NouveauContrat.SiteId == 0 || NouveauContrat.PosteId == 0)
        {
            MessageErreur = "Le site et le poste sont obligatoires pour créer un contrat.";
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var contrat = new Contrat { EmployeId = EmployeSelectionne.Id };
        NouveauContrat.AppliquerA(contrat);

        await unitOfWork.Contrats.AjouterAsync(contrat);
        await unitOfWork.EnregistrerAsync();

        await ChargerContratsAsync(EmployeSelectionne.Id);
        PreparerNouveauContrat();
    }

    [ObservableProperty]
    private PosteFiltreItem? _competenceAAjouter;

    [RelayCommand]
    private async Task AjouterCompetenceAsync()
    {
        if (EmployeSelectionne is null || CompetenceAAjouter is null)
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        await unitOfWork.EmployesPostes.AjouterAsync(new EmployePoste
        {
            EmployeId = EmployeSelectionne.Id,
            PosteId = CompetenceAAjouter.Id
        });
        await unitOfWork.EnregistrerAsync();

        await ChargerCompetencesAsync(EmployeSelectionne.Id);
    }

    public ObservableCollection<PosteFiltreItem> CompetencesEmploye { get; } = [];

    private async Task ChargerCompetencesAsync(int employeId)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var competences = await unitOfWork.EmployesPostes.ObtenirTousAsync();
        var postes = await unitOfWork.Postes.ObtenirTousAsync();
        var postesParId = postes.ToDictionary(p => p.Id, p => p.Nom);

        CompetencesEmploye.Clear();
        foreach (var competence in competences.Where(c => c.EmployeId == employeId))
        {
            CompetencesEmploye.Add(new PosteFiltreItem(competence.PosteId, postesParId.GetValueOrDefault(competence.PosteId, string.Empty)));
        }
    }

    [RelayCommand]
    private async Task RetirerCompetenceAsync(PosteFiltreItem competence)
    {
        if (EmployeSelectionne is null)
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var lien = await unitOfWork.EmployesPostes.ObtenirUnAsync(
            ep => ep.EmployeId == EmployeSelectionne.Id && ep.PosteId == competence.Id);

        if (lien is not null)
        {
            unitOfWork.EmployesPostes.Supprimer(lien);
            await unitOfWork.EnregistrerAsync();
        }

        await ChargerCompetencesAsync(EmployeSelectionne.Id);
    }

    [ObservableProperty]
    private IndisponibiliteEditableViewModel _nouvelleIndisponibilite = new();

    [RelayCommand]
    private async Task AjouterIndisponibiliteAsync()
    {
        if (EmployeSelectionne is null)
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var indisponibilite = new Indisponibilite { EmployeId = EmployeSelectionne.Id };
        NouvelleIndisponibilite.AppliquerA(indisponibilite);

        await unitOfWork.Indisponibilites.AjouterAsync(indisponibilite);
        await unitOfWork.EnregistrerAsync();

        await ChargerIndisponibilitesAsync(EmployeSelectionne.Id);
        NouvelleIndisponibilite = new IndisponibiliteEditableViewModel();
    }

    [RelayCommand]
    private async Task SupprimerIndisponibiliteAsync(IndisponibiliteEditableViewModel indisponibilite)
    {
        if (EmployeSelectionne is null)
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.Indisponibilites.ObtenirParIdAsync(indisponibilite.Id);
        if (entite is not null)
        {
            unitOfWork.Indisponibilites.Supprimer(entite);
            await unitOfWork.EnregistrerAsync();
        }

        await ChargerIndisponibilitesAsync(EmployeSelectionne.Id);
    }
}

public record SiteFiltreItem(int Id, string Nom)
{
    public override string ToString() => Nom;
}

public record PosteFiltreItem(int Id, string Nom)
{
    public override string ToString() => Nom;
}

public enum StatutFiltreItem
{
    Tous,
    Actif,
    Archive
}
