using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.App.Services;
using GestPlan.Core.Securite;
using GestPlan.Data.Securite;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Écran de connexion affiché au démarrage de l'application.
/// </summary>
public partial class ConnexionViewModel : ObservableObject
{
    private readonly IServiceAuthentification _serviceAuthentification;
    private readonly ISessionUtilisateurService _session;

    [ObservableProperty]
    private string _nomUtilisateur = string.Empty;

    [ObservableProperty]
    private string _motDePasse = string.Empty;

    [ObservableProperty]
    private string? _messageErreur;

    [ObservableProperty]
    private bool _connexionEnCours;

    public event EventHandler? ConnexionReussie;

    public ConnexionViewModel(IServiceAuthentification serviceAuthentification, ISessionUtilisateurService session)
    {
        _serviceAuthentification = serviceAuthentification;
        _session = session;
    }

    [RelayCommand]
    private async Task SeConnecterAsync()
    {
        MessageErreur = null;

        if (string.IsNullOrWhiteSpace(NomUtilisateur) || string.IsNullOrWhiteSpace(MotDePasse))
        {
            MessageErreur = "Veuillez saisir un identifiant et un mot de passe.";
            return;
        }

        ConnexionEnCours = true;
        try
        {
            var resultat = await _serviceAuthentification.AuthentifierAsync(NomUtilisateur, MotDePasse);

            switch (resultat.Statut)
            {
                case StatutAuthentification.Succes:
                    _session.OuvrirSession(resultat.Utilisateur!);
                    ConnexionReussie?.Invoke(this, EventArgs.Empty);
                    break;
                case StatutAuthentification.CompteInactif:
                    MessageErreur = "Ce compte a été désactivé.";
                    break;
                case StatutAuthentification.CompteVerrouille:
                    MessageErreur = "Ce compte est temporairement verrouillé suite à plusieurs échecs de connexion.";
                    break;
                case StatutAuthentification.MotDePasseExpire:
                    MessageErreur = "Votre mot de passe a expiré. Contactez un administrateur.";
                    break;
                default:
                    MessageErreur = "Identifiant ou mot de passe incorrect.";
                    break;
            }
        }
        finally
        {
            ConnexionEnCours = false;
        }
    }
}
