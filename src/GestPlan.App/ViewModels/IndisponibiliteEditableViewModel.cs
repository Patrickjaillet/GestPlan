using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation éditable d'une <see cref="Indisponibilite"/> pour la fiche employé.
/// </summary>
public partial class IndisponibiliteEditableViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty]
    private DayOfWeek _jourSemaine;

    [ObservableProperty]
    private TimeSpan _heureDebut;

    [ObservableProperty]
    private TimeSpan _heureFin;

    [ObservableProperty]
    private string? _motif;

    public IndisponibiliteEditableViewModel(Indisponibilite indisponibilite)
    {
        Id = indisponibilite.Id;
        _jourSemaine = indisponibilite.JourSemaine;
        _heureDebut = indisponibilite.HeureDebut.ToTimeSpan();
        _heureFin = indisponibilite.HeureFin.ToTimeSpan();
        _motif = indisponibilite.Motif;
    }

    public IndisponibiliteEditableViewModel()
    {
        _heureDebut = new TimeSpan(9, 0, 0);
        _heureFin = new TimeSpan(12, 0, 0);
    }

    public void AppliquerA(Indisponibilite indisponibilite)
    {
        indisponibilite.JourSemaine = JourSemaine;
        indisponibilite.HeureDebut = TimeOnly.FromTimeSpan(HeureDebut);
        indisponibilite.HeureFin = TimeOnly.FromTimeSpan(HeureFin);
        indisponibilite.Motif = Motif;
    }
}
