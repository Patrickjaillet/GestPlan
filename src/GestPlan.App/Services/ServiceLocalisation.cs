using System.IO;
using System.Text.Json;

namespace GestPlan.App.Services;

/// <inheritdoc cref="IServiceLocalisation"/>
public class ServiceLocalisation : IServiceLocalisation
{
    private readonly Dictionary<string, string> _chaines;

    public ServiceLocalisation()
    {
        var cheminFichier = Path.Combine(AppContext.BaseDirectory, "Resources", "i18n", "fr-FR.json");
        var json = File.ReadAllText(cheminFichier);

        _chaines = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                   ?? throw new InvalidOperationException("Le fichier de ressources i18n est vide ou invalide.");
    }

    public string ObtenirChaine(string cle) =>
        _chaines.TryGetValue(cle, out var valeur) ? valeur : $"[{cle}]";
}
