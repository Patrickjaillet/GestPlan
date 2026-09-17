using System.Text.RegularExpressions;

namespace GestPlan.Core.Securite;

/// <summary>
/// Règles de validité applicables à un mot de passe : longueur minimale, complexité,
/// et durée de validité optionnelle avant expiration.
/// </summary>
public class PolitiqueMotDePasse
{
    public int LongueurMinimum { get; set; } = 12;

    public bool ExigerMajuscule { get; set; } = true;

    public bool ExigerMinuscule { get; set; } = true;

    public bool ExigerChiffre { get; set; } = true;

    public bool ExigerCaractereSpecial { get; set; } = true;

    /// <summary>
    /// Durée de validité d'un mot de passe avant expiration. <c>null</c> désactive l'expiration.
    /// </summary>
    public TimeSpan? DureeValidite { get; set; }

    public IReadOnlyList<string> Valider(string motDePasse)
    {
        var erreurs = new List<string>();

        if (motDePasse.Length < LongueurMinimum)
        {
            erreurs.Add($"Le mot de passe doit contenir au moins {LongueurMinimum} caractères.");
        }

        if (ExigerMajuscule && !Regex.IsMatch(motDePasse, "[A-Z]"))
        {
            erreurs.Add("Le mot de passe doit contenir au moins une majuscule.");
        }

        if (ExigerMinuscule && !Regex.IsMatch(motDePasse, "[a-z]"))
        {
            erreurs.Add("Le mot de passe doit contenir au moins une minuscule.");
        }

        if (ExigerChiffre && !Regex.IsMatch(motDePasse, "[0-9]"))
        {
            erreurs.Add("Le mot de passe doit contenir au moins un chiffre.");
        }

        if (ExigerCaractereSpecial && !Regex.IsMatch(motDePasse, @"[^A-Za-z0-9]"))
        {
            erreurs.Add("Le mot de passe doit contenir au moins un caractère spécial.");
        }

        return erreurs;
    }

    public bool EstValide(string motDePasse) => Valider(motDePasse).Count == 0;

    public bool EstExpire(DateTime dateDernierChangement, DateTime maintenant) =>
        DureeValidite is not null && maintenant - dateDernierChangement > DureeValidite;
}
