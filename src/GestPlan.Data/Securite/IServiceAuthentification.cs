using GestPlan.Core.Securite;

namespace GestPlan.Data.Securite;

/// <summary>
/// Authentifie un utilisateur par nom d'utilisateur et mot de passe, en appliquant
/// le verrouillage après tentatives échouées et l'expiration de mot de passe.
/// </summary>
public interface IServiceAuthentification
{
    Task<ResultatAuthentification> AuthentifierAsync(
        string nomUtilisateur, string motDePasse, CancellationToken cancellationToken = default);

    Task DeconnecterAsync(int utilisateurId, CancellationToken cancellationToken = default);
}
