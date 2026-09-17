using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation éditable d'un <see cref="Employe"/> dans l'écran de gestion des employés.
/// </summary>
public partial class EmployeEditableViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty]
    private string _nom;

    [ObservableProperty]
    private string _prenom;

    [ObservableProperty]
    private DateTime? _dateNaissance;

    [ObservableProperty]
    private string? _email;

    [ObservableProperty]
    private string? _telephone;

    [ObservableProperty]
    private string? _adresse;

    [ObservableProperty]
    private string? _codePostal;

    [ObservableProperty]
    private string? _ville;

    [ObservableProperty]
    private string? _cheminPhoto;

    [ObservableProperty]
    private int _sitePrincipalId;

    [ObservableProperty]
    private bool _estArchive;

    public EmployeEditableViewModel(Employe employe)
    {
        Id = employe.Id;
        _nom = employe.Nom;
        _prenom = employe.Prenom;
        _dateNaissance = employe.DateNaissance?.ToDateTime(TimeOnly.MinValue);
        _email = employe.Email;
        _telephone = employe.Telephone;
        _adresse = employe.Adresse;
        _codePostal = employe.CodePostal;
        _ville = employe.Ville;
        _cheminPhoto = employe.CheminPhoto;
        _sitePrincipalId = employe.SitePrincipalId;
        _estArchive = employe.EstArchive;
    }

    public EmployeEditableViewModel()
    {
        _nom = string.Empty;
        _prenom = string.Empty;
    }

    public void AppliquerA(Employe employe)
    {
        employe.Nom = Nom;
        employe.Prenom = Prenom;
        employe.DateNaissance = DateNaissance is DateTime date ? DateOnly.FromDateTime(date) : null;
        employe.Email = Email;
        employe.Telephone = Telephone;
        employe.Adresse = Adresse;
        employe.CodePostal = CodePostal;
        employe.Ville = Ville;
        employe.CheminPhoto = CheminPhoto;
        employe.SitePrincipalId = SitePrincipalId;
        employe.EstArchive = EstArchive;
    }
}
