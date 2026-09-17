using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation éditable d'un <see cref="Utilisateur"/> dans l'écran de gestion des comptes.
/// </summary>
public partial class UtilisateurEditableViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty]
    private string _nomUtilisateur;

    [ObservableProperty]
    private string _nouveauMotDePasse = string.Empty;

    [ObservableProperty]
    private RoleUtilisateur _role;

    [ObservableProperty]
    private int? _siteAssigneId;

    [ObservableProperty]
    private bool _estActif;

    public UtilisateurEditableViewModel(Utilisateur utilisateur)
    {
        Id = utilisateur.Id;
        _nomUtilisateur = utilisateur.NomUtilisateur;
        _role = utilisateur.Role;
        _siteAssigneId = utilisateur.SiteAssigneId;
        _estActif = utilisateur.EstActif;
    }

    public UtilisateurEditableViewModel()
    {
        _nomUtilisateur = string.Empty;
        _role = RoleUtilisateur.Consultation;
        _estActif = true;
    }
}
