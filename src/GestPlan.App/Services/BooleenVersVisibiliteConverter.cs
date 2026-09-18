using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GestPlan.App.Services;

/// <summary>
/// Convertit un booléen en <see cref="Visibility"/> (vrai → visible, faux → masqué).
/// </summary>
public class BooleenVersVisibiliteConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
