using System.Globalization;
using System.Windows.Data;

namespace GestPlan.App.Services;

/// <summary>
/// Convertit une heure (<see cref="TimeOnly"/>) en position verticale en pixels sur la piste
/// horaire de la grille planning, à raison de <see cref="PixelsParHeure"/> pixels par heure,
/// démarrant à <see cref="HeureDebutGrille"/>.
/// </summary>
public class HeureVersPositionConverter : IValueConverter
{
    public const double PixelsParHeure = 60;
    public const int HeureDebutGrille = 6;

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is TimeOnly heure
            ? (heure.ToTimeSpan().TotalHours - HeureDebutGrille) * PixelsParHeure
            : 0d;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

/// <summary>
/// Convertit une durée exprimée par deux <see cref="TimeOnly"/> (début, fin) en hauteur en pixels.
/// </summary>
public class DureeVersHauteurConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is [TimeOnly debut, TimeOnly fin])
        {
            var heures = (fin.ToTimeSpan() - debut.ToTimeSpan()).TotalHours;
            return Math.Max(heures * HeureVersPositionConverter.PixelsParHeure, 16);
        }

        return 16d;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
