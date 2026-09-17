namespace GestPlan.Core.Securite;

/// <summary>
/// Hache et vérifie les mots de passe utilisateur.
/// </summary>
public interface IServiceHachageMotDePasse
{
    string Hacher(string motDePasse);

    bool Verifier(string motDePasse, string hash);
}
