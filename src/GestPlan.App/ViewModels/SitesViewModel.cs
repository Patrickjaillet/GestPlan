using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.App.Services;
using GestPlan.Core.Entites;
using GestPlan.Data.Repositories;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Écran de gestion des sites : création, modification, désactivation/réactivation,
/// et sélection du site global de l'application.
/// </summary>
public partial class SitesViewModel : PageViewModelBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ISelecteurSiteService _selecteurSite;

    [ObservableProperty]
    private SiteEditableViewModel _siteEnCoursEdition = new();

    [ObservableProperty]
    private int? _idEnCoursEdition;

    [ObservableProperty]
    private SiteEditableViewModel? _siteSelectionnePourPostes;

    [ObservableProperty]
    private PosteEditableViewModel _posteEnCoursEdition = new();

    public ObservableCollection<SiteEditableViewModel> Sites { get; } = [];

    public ObservableCollection<PosteEditableViewModel> PostesDuSite { get; } = [];

    public SitesViewModel(IUnitOfWorkFactory unitOfWorkFactory, ISelecteurSiteService selecteurSite)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _selecteurSite = selecteurSite;

        _ = ChargerAsync();
    }

    private async Task ChargerAsync()
    {
        Sites.Clear();

        using var unitOfWork = _unitOfWorkFactory.Creer();
        var sites = await unitOfWork.Sites.ObtenirTousAsync();
        foreach (var site in sites.OrderBy(s => s.Nom))
        {
            Sites.Add(new SiteEditableViewModel(site));
        }
    }

    [RelayCommand]
    private void PreparerNouveauSite()
    {
        IdEnCoursEdition = null;
        SiteEnCoursEdition = new SiteEditableViewModel();
    }

    [RelayCommand]
    private void ModifierSite(SiteEditableViewModel site)
    {
        IdEnCoursEdition = site.Id;
        SiteEnCoursEdition = new SiteEditableViewModel
        {
            Nom = site.Nom,
            Adresse = site.Adresse,
            CodePostal = site.CodePostal,
            Ville = site.Ville,
            HeureOuverture = site.HeureOuverture,
            HeureFermeture = site.HeureFermeture,
            FuseauHoraire = site.FuseauHoraire,
            EstActif = site.EstActif
        };
    }

    [RelayCommand]
    private async Task EnregistrerAsync()
    {
        if (string.IsNullOrWhiteSpace(SiteEnCoursEdition.Nom))
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        if (IdEnCoursEdition is int id)
        {
            var site = await unitOfWork.Sites.ObtenirParIdAsync(id);
            if (site is not null)
            {
                SiteEnCoursEdition.AppliquerA(site);
                unitOfWork.Sites.Modifier(site);
            }
        }
        else
        {
            var nouveauSite = new Site { Nom = SiteEnCoursEdition.Nom };
            SiteEnCoursEdition.AppliquerA(nouveauSite);
            await unitOfWork.Sites.AjouterAsync(nouveauSite);
        }

        await unitOfWork.EnregistrerAsync();
        await ChargerAsync();
        PreparerNouveauSite();
    }

    [RelayCommand]
    private async Task BasculerActivationAsync(SiteEditableViewModel site)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.Sites.ObtenirParIdAsync(site.Id);
        if (entite is null)
        {
            return;
        }

        entite.EstActif = !entite.EstActif;
        unitOfWork.Sites.Modifier(entite);
        await unitOfWork.EnregistrerAsync();
        await ChargerAsync();
    }

    [RelayCommand]
    private void SelectionnerSiteGlobal(SiteEditableViewModel site) =>
        _selecteurSite.SelectionnerSite(site.Id);

    [RelayCommand]
    private async Task SelectionnerSitePourPostesAsync(SiteEditableViewModel site)
    {
        SiteSelectionnePourPostes = site;
        PosteEnCoursEdition = new PosteEditableViewModel();
        await ChargerPostesAsync(site.Id);
    }

    private async Task ChargerPostesAsync(int siteId)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var postes = await unitOfWork.Postes.ObtenirTousAsync();

        PostesDuSite.Clear();
        foreach (var poste in postes.Where(p => p.SiteId == siteId).OrderBy(p => p.Nom))
        {
            PostesDuSite.Add(new PosteEditableViewModel(poste));
        }
    }

    [RelayCommand]
    private async Task AjouterPosteAsync()
    {
        if (SiteSelectionnePourPostes is null || string.IsNullOrWhiteSpace(PosteEnCoursEdition.Nom))
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var nouveauPoste = new Poste { Nom = PosteEnCoursEdition.Nom, SiteId = SiteSelectionnePourPostes.Id };
        PosteEnCoursEdition.AppliquerA(nouveauPoste);

        await unitOfWork.Postes.AjouterAsync(nouveauPoste);
        await unitOfWork.EnregistrerAsync();

        await ChargerPostesAsync(SiteSelectionnePourPostes.Id);
        PosteEnCoursEdition = new PosteEditableViewModel();
    }

    [RelayCommand]
    private async Task SupprimerPosteAsync(PosteEditableViewModel poste)
    {
        if (SiteSelectionnePourPostes is null)
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.Postes.ObtenirParIdAsync(poste.Id);
        if (entite is not null)
        {
            unitOfWork.Postes.Supprimer(entite);
            await unitOfWork.EnregistrerAsync();
        }

        await ChargerPostesAsync(SiteSelectionnePourPostes.Id);
    }
}
