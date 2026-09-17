using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;
using GestPlan.Core.Securite;
using GestPlan.Data.Repositories;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Écran de création du tout premier compte administrateur, affiché à la place de l'écran
/// de connexion lorsque la base ne contient encore aucun utilisateur.
/// </summary>
public partial class PremierCompteViewModel : ObservableObject
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IServiceHachageMotDePasse _hachage;
    private readonly PolitiqueMotDePasse _politiqueMotDePasse;

    [ObservableProperty]
    private string _nomUtilisateur = string.Empty;

    [ObservableProperty]
    private string _motDePasse = string.Empty;

    [ObservableProperty]
    private string _confirmationMotDePasse = string.Empty;

    [ObservableProperty]
    private string? _messageErreur;

    [ObservableProperty]
    private bool _creationEnCours;

    public event EventHandler<Utilisateur>? CompteCree;

    public PremierCompteViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IServiceHachageMotDePasse hachage,
        PolitiqueMotDePasse politiqueMotDePasse)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _hachage = hachage;
        _politiqueMotDePasse = politiqueMotDePasse;
    }

    [RelayCommand]
    private async Task CreerAsync()
    {
        MessageErreur = null;

        if (string.IsNullOrWhiteSpace(NomUtilisateur))
        {
            MessageErreur = "L'identifiant est obligatoire.";
            return;
        }

        if (MotDePasse != ConfirmationMotDePasse)
        {
            MessageErreur = "Les mots de passe ne correspondent pas.";
            return;
        }

        var erreurs = _politiqueMotDePasse.Valider(MotDePasse);
        if (erreurs.Count > 0)
        {
            MessageErreur = string.Join(" ", erreurs);
            return;
        }

        CreationEnCours = true;
        try
        {
            using var unitOfWork = _unitOfWorkFactory.Creer();

            var utilisateur = new Utilisateur
            {
                NomUtilisateur = NomUtilisateur,
                HashMotDePasse = _hachage.Hacher(MotDePasse),
                Role = RoleUtilisateur.Admin,
                EstActif = true
            };

            await unitOfWork.Utilisateurs.AjouterAsync(utilisateur);
            await unitOfWork.EnregistrerAsync();

            CompteCree?.Invoke(this, utilisateur);
        }
        finally
        {
            CreationEnCours = false;
        }
    }
}
