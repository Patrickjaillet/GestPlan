using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation éditable d'un <see cref="Contrat"/> pour la fiche employé.
/// </summary>
public partial class ContratEditableViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty]
    private int _siteId;

    [ObservableProperty]
    private int _posteId;

    [ObservableProperty]
    private TypeContrat _type;

    [ObservableProperty]
    private QuotiteTravail _quotite;

    [ObservableProperty]
    private double? _heuresHebdomadaires;

    [ObservableProperty]
    private double? _tauxHoraireBrut;

    [ObservableProperty]
    private DateTime _dateDebut = DateTime.Today;

    [ObservableProperty]
    private DateTime? _dateFin;

    public string NomSite { get; init; } = string.Empty;

    public string NomPoste { get; init; } = string.Empty;

    public bool EstEnCours => DateFin is null || DateFin >= DateTime.Today;

    public ContratEditableViewModel(Contrat contrat)
    {
        Id = contrat.Id;
        _siteId = contrat.SiteId;
        _posteId = contrat.PosteId;
        _type = contrat.Type;
        _quotite = contrat.Quotite;
        _heuresHebdomadaires = (double)contrat.HeuresHebdomadaires;
        _tauxHoraireBrut = (double)contrat.TauxHoraireBrut;
        _dateDebut = contrat.DateDebut.ToDateTime(TimeOnly.MinValue);
        _dateFin = contrat.DateFin?.ToDateTime(TimeOnly.MinValue);
        NomSite = contrat.Site?.Nom ?? string.Empty;
        NomPoste = contrat.Poste?.Nom ?? string.Empty;
    }

    public ContratEditableViewModel()
    {
    }

    public void AppliquerA(Contrat contrat)
    {
        contrat.SiteId = SiteId;
        contrat.PosteId = PosteId;
        contrat.Type = Type;
        contrat.Quotite = Quotite;
        contrat.HeuresHebdomadaires = (decimal)(HeuresHebdomadaires ?? 0);
        contrat.TauxHoraireBrut = (decimal)(TauxHoraireBrut ?? 0);
        contrat.DateDebut = DateOnly.FromDateTime(DateDebut);
        contrat.DateFin = DateFin is DateTime date ? DateOnly.FromDateTime(date) : null;
    }
}
