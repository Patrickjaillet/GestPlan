using System.Globalization;
using System.Windows.Data;

namespace GestPlan.App.Services;

/// <summary>
/// Inverse une valeur booléenne (utilisé pour désactiver un contrôle pendant un traitement en cours).
/// </summary>
public class BooleenInverseConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        !(value is true);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        !(value is true);
}
