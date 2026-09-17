namespace GestPlan.Core.Securite;

/// <inheritdoc cref="IServiceHachageMotDePasse"/>
public class ServiceHachageMotDePasse : IServiceHachageMotDePasse
{
    private const int CoutTravail = 12;

    public string Hacher(string motDePasse) =>
        BCrypt.Net.BCrypt.HashPassword(motDePasse, workFactor: CoutTravail);

    public bool Verifier(string motDePasse, string hash) =>
        BCrypt.Net.BCrypt.Verify(motDePasse, hash);
}
