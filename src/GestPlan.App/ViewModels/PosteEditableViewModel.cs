using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation éditable d'un <see cref="Poste"/> dans l'écran de gestion des sites.
/// </summary>
public partial class PosteEditableViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty]
    private string _nom;

    [ObservableProperty]
    private string? _couleurAffichage;

    public PosteEditableViewModel(Poste poste)
    {
        Id = poste.Id;
        _nom = poste.Nom;
        _couleurAffichage = poste.CouleurAffichage;
    }

    public PosteEditableViewModel()
    {
        _nom = string.Empty;
    }

    public void AppliquerA(Poste poste)
    {
        poste.Nom = Nom;
        poste.CouleurAffichage = CouleurAffichage;
    }
}
