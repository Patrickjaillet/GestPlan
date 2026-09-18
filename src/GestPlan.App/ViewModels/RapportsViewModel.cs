using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.App.Services;
using GestPlan.Core.Conformite;
using GestPlan.Core.Entites;
using GestPlan.Data.Export;
using GestPlan.Data.Repositories;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Écran de rapports : rapport de conformité légale (liste des anomalies détectées sur
/// une période pour un site) avec export PDF.
/// </summary>
public partial class RapportsViewModel : PageViewModelBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IServiceExportRapportConformite _serviceExport;

    [ObservableProperty]
    private Site? _siteSelectionne;

    [ObservableProperty]
    private DateTime _debutPeriode = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek == 0 ? 6 : (int)DateTime.Today.DayOfWeek - 1));

    [ObservableProperty]
    private DateTime _finPeriode = DateTime.Today.AddDays(7);

    [ObservableProperty]
    private string? _messageErreur;

    public ObservableCollection<Site> Sites { get; } = [];

    public ObservableCollection<AnomalieConformiteAffichable> Anomalies { get; } = [];

    public RapportsViewModel(IUnitOfWorkFactory unitOfWorkFactory, IServiceExportRapportConformite serviceExport)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _serviceExport = serviceExport;

        _ = ChargerSitesAsync();
    }

    private async Task ChargerSitesAsync()
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var sites = await unitOfWork.Sites.ObtenirTousAsync();
        Sites.Clear();
        foreach (var site in sites.Where(s => s.EstActif).OrderBy(s => s.Nom))
        {
            Sites.Add(site);
        }

        SiteSelectionne ??= Sites.FirstOrDefault();
        if (SiteSelectionne is not null)
        {
            await GenererRapportAsync();
        }
    }

    partial void OnSiteSelectionneChanged(Site? value) => _ = GenererRapportAsync();

    partial void OnDebutPeriodeChanged(DateTime value) => _ = GenererRapportAsync();

    partial void OnFinPeriodeChanged(DateTime value) => _ = GenererRapportAsync();

    [RelayCommand]
    private async Task GenererRapportAsync()
    {
        MessageErreur = null;
        Anomalies.Clear();

        if (SiteSelectionne is null)
        {
            return;
        }

        var debut = DateOnly.FromDateTime(DebutPeriode);
        var fin = DateOnly.FromDateTime(FinPeriode);

        if (fin < debut)
        {
            MessageErreur = "La date de fin doit être postérieure à la date de début.";
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var regles = await unitOfWork.ReglesConformite.ObtenirUnAsync(r => r.SiteId == SiteSelectionne.Id);
        if (regles is null)
        {
            MessageErreur = "Aucune règle de conformité n'est paramétrée pour ce site (voir l'écran Sites).";
            return;
        }

        var tousLesCreneaux = await unitOfWork.CreneauxPlanning.ObtenirTousAsync();
        var creneauxDuSite = tousLesCreneaux.Where(c => c.SiteId == SiteSelectionne.Id).ToList();

        var employes = await unitOfWork.Employes.ObtenirTousAsync();
        var employesParId = employes.ToDictionary(e => e.Id, e => $"{e.Nom} {e.Prenom}");

        var toutesLesAnomalies = new List<AnomalieConformiteAffichable>();

        foreach (var groupeEmploye in creneauxDuSite.GroupBy(c => c.EmployeId))
        {
            var anomaliesEmploye = MoteurConformite.Evaluer(groupeEmploye.Key, groupeEmploye, regles)
                .Where(a => a.Date >= debut && a.Date <= fin);

            var nomEmploye = employesParId.GetValueOrDefault(groupeEmploye.Key, string.Empty);

            foreach (var anomalie in anomaliesEmploye)
            {
                toutesLesAnomalies.Add(new AnomalieConformiteAffichable(anomalie.Type, nomEmploye, anomalie.Date, LibellesAnomalieConformite.Obtenir(anomalie.Type)));
            }
        }

        foreach (var anomalie in toutesLesAnomalies.OrderBy(a => a.NomEmploye).ThenBy(a => a.Date))
        {
            Anomalies.Add(anomalie);
        }
    }

    public byte[]? GenererExportPdf()
    {
        if (SiteSelectionne is null)
        {
            return null;
        }

        return _serviceExport.GenererPdf(
            SiteSelectionne.Nom,
            DateOnly.FromDateTime(DebutPeriode),
            DateOnly.FromDateTime(FinPeriode),
            Anomalies.ToList());
    }

}
