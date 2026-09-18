using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation d'un <see cref="CreneauPlanning"/> pour l'affichage et l'édition sur la grille.
/// </summary>
public partial class CreneauPlanningViewModel : ObservableObject
{
    public int Id { get; init; }

    public int EmployeId { get; init; }

    public string NomEmploye { get; init; } = string.Empty;

    public int SiteId { get; init; }

    public string NomSite { get; init; } = string.Empty;

    public int PosteId { get; init; }

    public string NomPoste { get; init; } = string.Empty;

    public string? CouleurPoste { get; init; }

    [ObservableProperty]
    private DateOnly _date;

    [ObservableProperty]
    private TimeOnly _heureDebut;

    [ObservableProperty]
    private TimeOnly _heureFin;

    [ObservableProperty]
    private StatutCreneau _statut;

    [ObservableProperty]
    private bool _estPublie;

    [ObservableProperty]
    private bool _estVerrouille;

    [ObservableProperty]
    private bool _enConflit;

    [ObservableProperty]
    private bool _enAnomalieConformite;

    [ObservableProperty]
    private string? _messageAnomaliesConformite;

    public static CreneauPlanningViewModel DepuisEntite(CreneauPlanning creneau, string nomEmploye, string nomSite, string nomPoste, string? couleurPoste) => new()
    {
        Id = creneau.Id,
        EmployeId = creneau.EmployeId,
        NomEmploye = nomEmploye,
        SiteId = creneau.SiteId,
        NomSite = nomSite,
        PosteId = creneau.PosteId,
        NomPoste = nomPoste,
        CouleurPoste = couleurPoste,
        Date = creneau.Date,
        HeureDebut = creneau.HeureDebut,
        HeureFin = creneau.HeureFin,
        Statut = creneau.Statut,
        EstPublie = creneau.EstPublie
    };
}
