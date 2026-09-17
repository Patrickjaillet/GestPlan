using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.App.Services;
using GestPlan.Core.Entites;
using GestPlan.Core.Securite;
using GestPlan.Data.Repositories;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Écran de gestion des comptes utilisateurs, réservé aux administrateurs :
/// création, modification (dont changement de mot de passe), désactivation/réactivation.
/// </summary>
public partial class UtilisateursViewModel : PageViewModelBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IServiceHachageMotDePasse _hachage;
    private readonly PolitiqueMotDePasse _politiqueMotDePasse;

    [ObservableProperty]
    private UtilisateurEditableViewModel _utilisateurEnCoursEdition = new();

    [ObservableProperty]
    private int? _idEnCoursEdition;

    [ObservableProperty]
    private string? _messageErreur;

    public bool AccesAutorise { get; }

    public ObservableCollection<UtilisateurEditableViewModel> Utilisateurs { get; } = [];

    public UtilisateursViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IServiceHachageMotDePasse hachage,
        PolitiqueMotDePasse politiqueMotDePasse,
        ISessionUtilisateurService session)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _hachage = hachage;
        _politiqueMotDePasse = politiqueMotDePasse;

        AccesAutorise = session.APourRoleMinimum(GestPlan.Core.Enumerations.RoleUtilisateur.Admin);

        if (AccesAutorise)
        {
            _ = ChargerAsync();
        }
    }

    private async Task ChargerAsync()
    {
        Utilisateurs.Clear();

        using var unitOfWork = _unitOfWorkFactory.Creer();
        var utilisateurs = await unitOfWork.Utilisateurs.ObtenirTousAsync();
        foreach (var utilisateur in utilisateurs.OrderBy(u => u.NomUtilisateur))
        {
            Utilisateurs.Add(new UtilisateurEditableViewModel(utilisateur));
        }
    }

    [RelayCommand]
    private void PreparerNouvelUtilisateur()
    {
        IdEnCoursEdition = null;
        UtilisateurEnCoursEdition = new UtilisateurEditableViewModel();
        MessageErreur = null;
    }

    [RelayCommand]
    private void ModifierUtilisateur(UtilisateurEditableViewModel utilisateur)
    {
        IdEnCoursEdition = utilisateur.Id;
        UtilisateurEnCoursEdition = new UtilisateurEditableViewModel
        {
            NomUtilisateur = utilisateur.NomUtilisateur,
            Role = utilisateur.Role,
            SiteAssigneId = utilisateur.SiteAssigneId,
            EstActif = utilisateur.EstActif
        };
        MessageErreur = null;
    }

    [RelayCommand]
    private async Task EnregistrerAsync()
    {
        MessageErreur = null;

        if (string.IsNullOrWhiteSpace(UtilisateurEnCoursEdition.NomUtilisateur))
        {
            MessageErreur = "L'identifiant est obligatoire.";
            return;
        }

        var creationCompte = IdEnCoursEdition is null;

        if (creationCompte || !string.IsNullOrEmpty(UtilisateurEnCoursEdition.NouveauMotDePasse))
        {
            var erreurs = _politiqueMotDePasse.Valider(UtilisateurEnCoursEdition.NouveauMotDePasse);
            if (erreurs.Count > 0)
            {
                MessageErreur = string.Join(" ", erreurs);
                return;
            }
        }

        using var unitOfWork = _unitOfWorkFactory.Creer();

        if (IdEnCoursEdition is int id)
        {
            var utilisateur = await unitOfWork.Utilisateurs.ObtenirParIdAsync(id);
            if (utilisateur is null)
            {
                return;
            }

            utilisateur.NomUtilisateur = UtilisateurEnCoursEdition.NomUtilisateur;
            utilisateur.Role = UtilisateurEnCoursEdition.Role;
            utilisateur.SiteAssigneId = UtilisateurEnCoursEdition.SiteAssigneId;
            utilisateur.EstActif = UtilisateurEnCoursEdition.EstActif;

            if (!string.IsNullOrEmpty(UtilisateurEnCoursEdition.NouveauMotDePasse))
            {
                utilisateur.HashMotDePasse = _hachage.Hacher(UtilisateurEnCoursEdition.NouveauMotDePasse);
                utilisateur.DateDernierChangementMotDePasse = DateTime.UtcNow;
            }

            unitOfWork.Utilisateurs.Modifier(utilisateur);
        }
        else
        {
            var nouvelUtilisateur = new Utilisateur
            {
                NomUtilisateur = UtilisateurEnCoursEdition.NomUtilisateur,
                HashMotDePasse = _hachage.Hacher(UtilisateurEnCoursEdition.NouveauMotDePasse),
                Role = UtilisateurEnCoursEdition.Role,
                SiteAssigneId = UtilisateurEnCoursEdition.SiteAssigneId,
                EstActif = UtilisateurEnCoursEdition.EstActif
            };

            await unitOfWork.Utilisateurs.AjouterAsync(nouvelUtilisateur);
        }

        await unitOfWork.EnregistrerAsync();
        await ChargerAsync();
        PreparerNouvelUtilisateur();
    }

    [RelayCommand]
    private async Task BasculerActivationAsync(UtilisateurEditableViewModel utilisateur)
    {
        using var unitOfWork = _unitOfWorkFactory.Creer();

        var entite = await unitOfWork.Utilisateurs.ObtenirParIdAsync(utilisateur.Id);
        if (entite is null)
        {
            return;
        }

        entite.EstActif = !entite.EstActif;
        unitOfWork.Utilisateurs.Modifier(entite);
        await unitOfWork.EnregistrerAsync();
        await ChargerAsync();
    }
}
