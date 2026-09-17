using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GestPlan.App.Services;

/// <summary>
/// Rend un élément visible uniquement si l'objet lié n'est pas nul
/// (utilisé pour n'afficher une fiche détaillée que lorsqu'un élément est sélectionné).
/// </summary>
public class ObjetNonNulVersVisibiliteConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
