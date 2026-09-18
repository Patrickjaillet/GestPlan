using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.App.Services;
using GestPlan.Core.Conges;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;
using GestPlan.Core.Securite;
using GestPlan.Data.Export;
using GestPlan.Data.Repositories;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Écran de gestion des congés et absences : demande, workflow de validation, solde,
/// calendrier consolidé, types d'absence paramétrables (réservé Admin), export PDF.
/// </summary>
public partial class CongesViewModel : PageViewModelBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ISessionUtilisateurService _session;
    private readonly IServiceExportSoldeConges _serviceExport;

    [ObservableProperty]
    private Employe? _employeSelectionne;

    [ObservableProperty]
    private int _anneeSelectionnee = DateTime.Today.Year;

    [ObservableProperty]
    private SoldeConges _soldeAffiche = new(0, 0, 0);

    [ObservableProperty]
    private string? _messageErreur;

    [ObservableProperty]
    private TypeAbsenceEditableViewModel? _typeAbsenceSelectionne;

    [ObservableProperty]
    private DateTime _nouvelleDateDebut = DateTime.Today;

    [ObservableProperty]
    private DateTime _nouvelleDateFin = DateTime.Today;

    [ObservableProperty]
    private string? _nouveauMotif;

    [ObservableProperty]
    private TypeAbsenceEditableViewModel _nouveauType = new();

    public bool GestionTypesAutorisee { get; }

    public ObservableCollection<Employe> Employes { get; } = [];

    public ObservableCollection<TypeAbsenceEditableViewModel> TypesAbsence { get; } = [];

    public ObservableCollection<AbsenceEditableViewModel> Absences { get; } = [];

    public ObservableCollection<AbsenceEditableViewModel> AbsencesCalendrier { get; } = [];

    public CongesViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        ISessionUtilisateurService session,
        IServiceExportSoldeConges serviceExport)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _session = session;
        _serviceExport = serviceExport;

        GestionTypesAutorisee = session.APourRoleMinimum(RoleUtilisateur.Admin);

        _ = ChargerAsync();
    }

    private async Task ChargerAsync()
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var employes = await unitOfWork.Employes.ObtenirTousAsync();
        Employes.Clear();
        foreach (var employe in employes.Where(e => !e.EstArchive).OrderBy(e => e.Nom).ThenBy(e => e.Prenom))
        {
            Employes.Add(employe);
        }

        EmployeSelectionne ??= Employes.FirstOrDefault();

        var types = await unitOfWork.TypesAbsence.ObtenirTousAsync();
        TypesAbsence.Clear();
        foreach (var type in types.Where(t => t.EstActif).OrderBy(t => t.Nom))
        {
            TypesAbsence.Add(new TypeAbsenceEditableViewModel(type));
        }
        NouveauType = new TypeAbsenceEditableViewModel();

        await ChargerAbsencesAsync();
        await ChargerCalendrierAsync();
    }

    partial void OnEmployeSelectionneChanged(Employe? value) => _ = ChargerAbsencesAsync();

    partial void OnAnneeSelectionneeChanged(int value) => _ = ChargerAbsencesAsync();

    private async Task ChargerAbsencesAsync()
    {
        if (EmployeSelectionne is null)
        {
            Absences.Clear();
            SoldeAffiche = new SoldeConges(0, 0, 0);
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var toutesLesAbsences = await unitOfWork.Absences.ObtenirTousAsync();
        var typesAbsence = await unitOfWork.TypesAbsence.ObtenirTousAsync();
        var typesParId = typesAbsence.ToDictionary(t => t.Id, t => t);

        var absencesEmploye = toutesLesAbsences.Where(a => a.EmployeId == EmployeSelectionne.Id).ToList();
        foreach (var absence in absencesEmploye)
        {
            absence.TypeAbsence = typesParId[absence.TypeAbsenceId];
        }

        Absences.Clear();
        foreach (var absence in absencesEmploye.OrderByDescending(a => a.DateDebut))
        {
            Absences.Add(AbsenceEditableViewModel.DepuisEntite(absence, $"{EmployeSelectionne.Nom} {EmployeSelectionne.Prenom}", absence.TypeAbsence.Nom));
        }

        var debutPeriode = new DateOnly(AnneeSelectionnee, 1, 1);
        var finPeriode = new DateOnly(AnneeSelectionnee, 12, 31);

        var reglesAcquisition = await unitOfWork.ReglesAcquisitionConges.ObtenirUnAsync(r => r.SiteId == EmployeSelectionne.SitePrincipalId);
        var joursAcquisParMois = reglesAcquisition?.JoursAcquisParMois ?? 2.5m;

        var contrats = await unitOfWork.Contrats.ObtenirTousAsync();
        var dateEntree = contrats
            .Where(c => c.EmployeId == EmployeSelectionne.Id)
            .Select(c => c.DateDebut)
            .DefaultIfEmpty(debutPeriode)
            .Min();

        var absencesDeLaPeriode = absencesEmploye
            .Where(a => a.DateDebut <= finPeriode && a.DateFin >= debutPeriode)
            .ToList();

        SoldeAffiche = CalculateurSoldeConges.Calculer(debutPeriode, finPeriode, dateEntree, joursAcquisParMois, absencesDeLaPeriode);
    }

    private async Task ChargerCalendrierAsync()
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var toutesLesAbsences = await unitOfWork.Absences.ObtenirTousAsync();
        var employes = await unitOfWork.Employes.ObtenirTousAsync();
        var typesAbsence = await unitOfWork.TypesAbsence.ObtenirTousAsync();

        var employesParId = employes.ToDictionary(e => e.Id, e => $"{e.Nom} {e.Prenom}");
        var typesParId = typesAbsence.ToDictionary(t => t.Id, t => t.Nom);

        AbsencesCalendrier.Clear();
        foreach (var absence in toutesLesAbsences
                     .Where(a => a.Statut == StatutAbsence.Validee)
                     .OrderBy(a => a.DateDebut))
        {
            AbsencesCalendrier.Add(AbsenceEditableViewModel.DepuisEntite(
                absence,
                employesParId.GetValueOrDefault(absence.EmployeId, string.Empty),
                typesParId.GetValueOrDefault(absence.TypeAbsenceId, string.Empty)));
        }
    }

    [RelayCommand]
    private async Task DemanderAbsenceAsync()
    {
        MessageErreur = null;

        if (EmployeSelectionne is null || TypeAbsenceSelectionne is null)
        {
            MessageErreur = "Veuillez sélectionner un employé et un type d'absence.";
            return;
        }

        var nouvelleAbsence = new Absence
        {
            EmployeId = EmployeSelectionne.Id,
            SiteId = EmployeSelectionne.SitePrincipalId,
            TypeAbsenceId = TypeAbsenceSelectionne.Id,
            DateDebut = DateOnly.FromDateTime(NouvelleDateDebut),
            DateFin = DateOnly.FromDateTime(NouvelleDateFin),
            Motif = NouveauMotif,
            Statut = StatutAbsence.Demandee
        };

        var erreursCoherence = ValidateurAbsence.Valider(nouvelleAbsence);
        if (erreursCoherence.Count > 0)
        {
            MessageErreur = string.Join(" ", erreursCoherence);
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        var absencesExistantes = await unitOfWork.Absences.ObtenirTousAsync();
        if (ValidateurAbsence.ChevaucheUneAutreAbsence(nouvelleAbsence, absencesExistantes))
        {
            MessageErreur = "Cette absence chevauche une autre absence déjà existante pour cet employé.";
            return;
        }

        var creneaux = await unitOfWork.CreneauxPlanning.ObtenirTousAsync();
        if (ValidateurAbsence.ChevaucheUnCreneauPlanning(nouvelleAbsence, creneaux))
        {
            var nombreCreneaux = creneaux.Count(c =>
                c.EmployeId == nouvelleAbsence.EmployeId &&
                nouvelleAbsence.DateDebut <= c.Date && c.Date <= nouvelleAbsence.DateFin);
            MessageErreur = $"Attention : {nombreCreneaux} créneau(x) de planning existent déjà sur cette période et seront en conflit.";
        }

        await unitOfWork.Absences.AjouterAsync(nouvelleAbsence);
        await unitOfWork.EnregistrerAsync();

        await ChargerAbsencesAsync();
        await ChargerCalendrierAsync();

        NouveauMotif = null;
    }

    [RelayCommand]
    private async Task ValiderAbsenceAsync(AbsenceEditableViewModel absence)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.Absences.ObtenirParIdAsync(absence.Id);
        if (entite is null)
        {
            return;
        }

        entite.Statut = StatutAbsence.Validee;
        entite.ValideParUtilisateurId = _session.UtilisateurConnecte?.Id;
        entite.DateValidation = DateTime.UtcNow;
        unitOfWork.Absences.Modifier(entite);
        await unitOfWork.EnregistrerAsync();

        await ChargerAbsencesAsync();
        await ChargerCalendrierAsync();
    }

    [RelayCommand]
    private async Task RefuserAbsenceAsync(AbsenceEditableViewModel absence)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.Absences.ObtenirParIdAsync(absence.Id);
        if (entite is null)
        {
            return;
        }

        entite.Statut = StatutAbsence.Refusee;
        entite.ValideParUtilisateurId = _session.UtilisateurConnecte?.Id;
        entite.DateValidation = DateTime.UtcNow;
        unitOfWork.Absences.Modifier(entite);
        await unitOfWork.EnregistrerAsync();

        await ChargerAbsencesAsync();
        await ChargerCalendrierAsync();
    }

    [RelayCommand]
    private async Task AjouterTypeAbsenceAsync()
    {
        if (string.IsNullOrWhiteSpace(NouveauType.Nom))
        {
            return;
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        await unitOfWork.TypesAbsence.AjouterAsync(new TypeAbsence
        {
            Nom = NouveauType.Nom,
            DecompteDuSolde = NouveauType.DecompteDuSolde
        });
        await unitOfWork.EnregistrerAsync();

        await ChargerAsync();
    }

    public byte[]? GenererExportPdf()
    {
        if (EmployeSelectionne is null)
        {
            return null;
        }

        var absencesDeLAnnee = Absences
            .Where(a => a.DateDebut.Year == AnneeSelectionnee || a.DateFin.Year == AnneeSelectionnee)
            .Select(vm => new Absence
            {
                DateDebut = DateOnly.FromDateTime(vm.DateDebut),
                DateFin = DateOnly.FromDateTime(vm.DateFin),
                Statut = vm.Statut,
                TypeAbsence = new TypeAbsence { Nom = vm.NomTypeAbsence }
            })
            .ToList();

        return _serviceExport.GenererPdf(EmployeSelectionne, AnneeSelectionnee, SoldeAffiche, absencesDeLAnnee);
    }
}
