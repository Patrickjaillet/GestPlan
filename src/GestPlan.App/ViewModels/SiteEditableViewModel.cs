using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation éditable d'un <see cref="Site"/> dans l'écran de gestion des sites.
/// </summary>
public partial class SiteEditableViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty]
    private string _nom;

    [ObservableProperty]
    private string? _adresse;

    [ObservableProperty]
    private string? _codePostal;

    [ObservableProperty]
    private string? _ville;

    [ObservableProperty]
    private TimeOnly? _heureOuverture;

    [ObservableProperty]
    private TimeOnly? _heureFermeture;

    [ObservableProperty]
    private string _fuseauHoraire;

    [ObservableProperty]
    private bool _estActif;

    public SiteEditableViewModel(Site site)
    {
        Id = site.Id;
        _nom = site.Nom;
        _adresse = site.Adresse;
        _codePostal = site.CodePostal;
        _ville = site.Ville;
        _heureOuverture = site.HeureOuverture;
        _heureFermeture = site.HeureFermeture;
        _fuseauHoraire = site.FuseauHoraire;
        _estActif = site.EstActif;
    }

    public SiteEditableViewModel()
    {
        _nom = string.Empty;
        _fuseauHoraire = "Europe/Paris";
        _estActif = true;
    }

    public void AppliquerA(Site site)
    {
        site.Nom = Nom;
        site.Adresse = Adresse;
        site.CodePostal = CodePostal;
        site.Ville = Ville;
        site.HeureOuverture = HeureOuverture;
        site.HeureFermeture = HeureFermeture;
        site.FuseauHoraire = FuseauHoraire;
        site.EstActif = EstActif;
    }
}
