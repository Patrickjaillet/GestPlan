using System.Windows.Markup;

namespace GestPlan.App.Services;

/// <summary>
/// Extension de balisage XAML permettant de lier une chaîne d'interface à sa clé i18n :
/// <c>{loc:Localisation Cle=Navigation.Planning}</c>. Résout le service de localisation
/// via le conteneur de dépendances de l'application.
/// </summary>
[MarkupExtensionReturnType(typeof(string))]
public class LocalisationExtension : MarkupExtension
{
    public required string Cle { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        App.Services.GetService(typeof(IServiceLocalisation)) is IServiceLocalisation service
            ? service.ObtenirChaine(Cle)
            : $"[{Cle}]";
}
