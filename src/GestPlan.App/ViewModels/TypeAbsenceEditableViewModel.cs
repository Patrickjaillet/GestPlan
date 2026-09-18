using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation éditable d'un <see cref="TypeAbsence"/>.
/// </summary>
public partial class TypeAbsenceEditableViewModel : ObservableObject
{
    public int Id { get; init; }

    [ObservableProperty]
    private string _nom;

    [ObservableProperty]
    private bool _decompteDuSolde;

    public TypeAbsenceEditableViewModel(TypeAbsence typeAbsence)
    {
        Id = typeAbsence.Id;
        _nom = typeAbsence.Nom;
        _decompteDuSolde = typeAbsence.DecompteDuSolde;
    }

    public TypeAbsenceEditableViewModel()
    {
        _nom = string.Empty;
    }

    public override string ToString() => Nom;
}
