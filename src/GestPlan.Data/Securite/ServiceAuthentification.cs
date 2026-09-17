using GestPlan.Core.Entites;
using GestPlan.Core.Securite;
using GestPlan.Data.Repositories;

namespace GestPlan.Data.Securite;

/// <inheritdoc cref="IServiceAuthentification"/>
public class ServiceAuthentification(
    IUnitOfWorkFactory unitOfWorkFactory,
    IServiceHachageMotDePasse hachage,
    PolitiqueMotDePasse politiqueMotDePasse) : IServiceAuthentification
{
    private const int TentativesMaximalesAvantVerrouillage = 5;
    private static readonly TimeSpan DureeVerrouillage = TimeSpan.FromMinutes(15);

    public async Task<ResultatAuthentification> AuthentifierAsync(
        string nomUtilisateur, string motDePasse, CancellationToken cancellationToken = default)
    {
        using var unitOfWork = unitOfWorkFactory.Creer();

        var utilisateur = await unitOfWork.Utilisateurs.ObtenirUnAsync(
            u => u.NomUtilisateur == nomUtilisateur, cancellationToken);

        var maintenant = DateTime.UtcNow;

        if (utilisateur is null)
        {
            return ResultatAuthentification.Echoue(StatutAuthentification.IdentifiantsInvalides);
        }

        if (utilisateur.VerrouilleJusqua is DateTime verrouilleJusqua && verrouilleJusqua > maintenant)
        {
            return ResultatAuthentification.Echoue(StatutAuthentification.CompteVerrouille);
        }

        if (!hachage.Verifier(motDePasse, utilisateur.HashMotDePasse))
        {
            utilisateur.TentativesEchoueesConsecutives++;
            if (utilisateur.TentativesEchoueesConsecutives >= TentativesMaximalesAvantVerrouillage)
            {
                utilisateur.VerrouilleJusqua = maintenant.Add(DureeVerrouillage);
            }

            unitOfWork.Utilisateurs.Modifier(utilisateur);
            await unitOfWork.EnregistrerAsync(cancellationToken);
            return ResultatAuthentification.Echoue(StatutAuthentification.IdentifiantsInvalides);
        }

        if (!utilisateur.EstActif)
        {
            return ResultatAuthentification.Echoue(StatutAuthentification.CompteInactif);
        }

        if (politiqueMotDePasse.EstExpire(utilisateur.DateDernierChangementMotDePasse, maintenant))
        {
            return ResultatAuthentification.Echoue(StatutAuthentification.MotDePasseExpire);
        }

        utilisateur.TentativesEchoueesConsecutives = 0;
        utilisateur.VerrouilleJusqua = null;
        utilisateur.DerniereConnexion = maintenant;
        unitOfWork.Utilisateurs.Modifier(utilisateur);

        await unitOfWork.JournauxAudit.AjouterAsync(new JournalAudit
        {
            DateAction = maintenant,
            UtilisateurId = utilisateur.Id,
            NomEntite = "Connexion",
            CleEntite = utilisateur.Id.ToString(),
            Action = TypeActionAudit.Creation
        }, cancellationToken);

        await unitOfWork.EnregistrerAsync(cancellationToken);

        return ResultatAuthentification.Reussi(utilisateur);
    }

    public async Task DeconnecterAsync(int utilisateurId, CancellationToken cancellationToken = default)
    {
        using var unitOfWork = unitOfWorkFactory.Creer();

        await unitOfWork.JournauxAudit.AjouterAsync(new JournalAudit
        {
            DateAction = DateTime.UtcNow,
            UtilisateurId = utilisateurId,
            NomEntite = "Deconnexion",
            CleEntite = utilisateurId.ToString(),
            Action = TypeActionAudit.Creation
        }, cancellationToken);

        await unitOfWork.EnregistrerAsync(cancellationToken);
    }
}
