using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GestPlan.App.Services;

/// <summary>
/// Rend un élément visible uniquement si la chaîne liée n'est ni vide ni nulle
/// (utilisé pour n'afficher un message d'erreur que lorsqu'il y en a un).
/// </summary>
public class ChaineVersVisibiliteConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
