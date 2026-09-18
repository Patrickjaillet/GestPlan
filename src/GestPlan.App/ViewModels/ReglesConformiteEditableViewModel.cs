using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation éditable des <see cref="ReglesConformite"/> d'un site.
/// </summary>
public partial class ReglesConformiteEditableViewModel : ObservableObject
{
    [ObservableProperty]
    private double _reposQuotidienMinimumHeures;

    [ObservableProperty]
    private double _reposHebdomadaireMinimumHeures;

    [ObservableProperty]
    private double _dureeMaximaleQuotidienneHeures;

    [ObservableProperty]
    private double _dureeMaximaleHebdomadaireHeures;

    [ObservableProperty]
    private double _dureeTravailContinuAvantPauseHeures;

    [ObservableProperty]
    private double _dureePauseObligatoireMinutes;

    [ObservableProperty]
    private int _joursConsecutifsMaximum;

    [ObservableProperty]
    private bool _bloquerPublicationSiNonConforme;

    public ReglesConformiteEditableViewModel(ReglesConformite regles)
    {
        _reposQuotidienMinimumHeures = regles.ReposQuotidienMinimum.TotalHours;
        _reposHebdomadaireMinimumHeures = regles.ReposHebdomadaireMinimum.TotalHours;
        _dureeMaximaleQuotidienneHeures = regles.DureeMaximaleQuotidienne.TotalHours;
        _dureeMaximaleHebdomadaireHeures = regles.DureeMaximaleHebdomadaire.TotalHours;
        _dureeTravailContinuAvantPauseHeures = regles.DureeTravailContinuAvantPause.TotalHours;
        _dureePauseObligatoireMinutes = regles.DureePauseObligatoire.TotalMinutes;
        _joursConsecutifsMaximum = regles.JoursConsecutifsMaximum;
        _bloquerPublicationSiNonConforme = regles.BloquerPublicationSiNonConforme;
    }

    public ReglesConformiteEditableViewModel()
    {
        var defaut = new ReglesConformite();
        _reposQuotidienMinimumHeures = defaut.ReposQuotidienMinimum.TotalHours;
        _reposHebdomadaireMinimumHeures = defaut.ReposHebdomadaireMinimum.TotalHours;
        _dureeMaximaleQuotidienneHeures = defaut.DureeMaximaleQuotidienne.TotalHours;
        _dureeMaximaleHebdomadaireHeures = defaut.DureeMaximaleHebdomadaire.TotalHours;
        _dureeTravailContinuAvantPauseHeures = defaut.DureeTravailContinuAvantPause.TotalHours;
        _dureePauseObligatoireMinutes = defaut.DureePauseObligatoire.TotalMinutes;
        _joursConsecutifsMaximum = defaut.JoursConsecutifsMaximum;
    }

    public void AppliquerA(ReglesConformite regles)
    {
        regles.ReposQuotidienMinimum = TimeSpan.FromHours(ReposQuotidienMinimumHeures);
        regles.ReposHebdomadaireMinimum = TimeSpan.FromHours(ReposHebdomadaireMinimumHeures);
        regles.DureeMaximaleQuotidienne = TimeSpan.FromHours(DureeMaximaleQuotidienneHeures);
        regles.DureeMaximaleHebdomadaire = TimeSpan.FromHours(DureeMaximaleHebdomadaireHeures);
        regles.DureeTravailContinuAvantPause = TimeSpan.FromHours(DureeTravailContinuAvantPauseHeures);
        regles.DureePauseObligatoire = TimeSpan.FromMinutes(DureePauseObligatoireMinutes);
        regles.JoursConsecutifsMaximum = JoursConsecutifsMaximum;
        regles.BloquerPublicationSiNonConforme = BloquerPublicationSiNonConforme;
    }
}
