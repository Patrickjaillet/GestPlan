using System.Globalization;
using System.Windows.Data;

namespace GestPlan.App.Services;

/// <summary>
/// Concatène une liste d'erreurs de validation en une seule chaîne affichable.
/// </summary>
public class ListeErreursConverter : IValueConverter
{
    public static readonly ListeErreursConverter Instance = new();

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is IEnumerable<string> erreurs ? string.Join(" ", erreurs) : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
