using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GestPlan.App.Services;

/// <summary>
/// Convertit une couleur hexadécimale (ex. <c>#3B7CF2</c>) en <see cref="Brush"/> pour le XAML.
/// Retourne une couleur neutre par défaut si la valeur est absente ou invalide.
/// </summary>
public class CouleurHexVersBrushConverter : IValueConverter
{
    private static readonly Brush CouleurParDefaut = new SolidColorBrush(Color.FromRgb(0x64, 0x64, 0x64));

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string couleurHex && !string.IsNullOrWhiteSpace(couleurHex))
        {
            try
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(couleurHex));
            }
            catch (FormatException)
            {
                return CouleurParDefaut;
            }
        }

        return CouleurParDefaut;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
