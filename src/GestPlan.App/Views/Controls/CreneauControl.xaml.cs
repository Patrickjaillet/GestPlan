using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GestPlan.App.ViewModels;

namespace GestPlan.App.Views.Controls;

/// <summary>
/// Représentation visuelle et interactive d'un créneau sur la grille planning :
/// déplacement par glisser-déposer (via <see cref="DragDrop"/>) et redimensionnement
/// par les poignées haut/bas (via <see cref="Thumb"/>).
/// </summary>
public partial class CreneauControl : UserControl
{
    public static readonly RoutedEvent RedimensionneEvent = EventManager.RegisterRoutedEvent(
        nameof(Redimensionne), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CreneauControl));

    public event RoutedEventHandler Redimensionne
    {
        add => AddHandler(RedimensionneEvent, value);
        remove => RemoveHandler(RedimensionneEvent, value);
    }

    private Point _pointDeDepart;
    private bool _glissementEnCours;
    private double _decalageHautCumule;
    private double _decalageBasCumule;

    public CreneauControl()
    {
        InitializeComponent();
    }

    private CreneauPlanningViewModel? ViewModel => DataContext as CreneauPlanningViewModel;

    private void Corps_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (ViewModel?.EstVerrouille == true)
        {
            return;
        }

        _pointDeDepart = e.GetPosition(null);
        _glissementEnCours = false;
    }

    private void Corps_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || ViewModel is null || ViewModel.EstVerrouille)
        {
            return;
        }

        var positionActuelle = e.GetPosition(null);
        var deplacement = _pointDeDepart - positionActuelle;

        if (!_glissementEnCours &&
            (Math.Abs(deplacement.X) > SystemParameters.MinimumHorizontalDragDistance ||
             Math.Abs(deplacement.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
            _glissementEnCours = true;
            DragDrop.DoDragDrop(this, ViewModel, DragDropEffects.Move);
        }
    }

    private void Corps_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _glissementEnCours = false;

    private void PoigneeHaut_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (ViewModel is null || ViewModel.EstVerrouille)
        {
            return;
        }

        _decalageHautCumule += e.VerticalChange;
        var minutes = ArrondirAuQuartDHeure(_decalageHautCumule);

        var nouvelleHeureDebut = ViewModel.HeureDebut.AddMinutes(minutes);
        if (nouvelleHeureDebut < ViewModel.HeureFin)
        {
            ViewModel.HeureDebut = nouvelleHeureDebut;
            _decalageHautCumule = 0;
        }
    }

    private void PoigneeBas_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (ViewModel is null || ViewModel.EstVerrouille)
        {
            return;
        }

        _decalageBasCumule += e.VerticalChange;
        var minutes = ArrondirAuQuartDHeure(_decalageBasCumule);

        var nouvelleHeureFin = ViewModel.HeureFin.AddMinutes(minutes);
        if (nouvelleHeureFin > ViewModel.HeureDebut)
        {
            ViewModel.HeureFin = nouvelleHeureFin;
            _decalageBasCumule = 0;
        }
    }

    private static int ArrondirAuQuartDHeure(double pixels)
    {
        var minutesParPixel = 60.0 / Services.HeureVersPositionConverter.PixelsParHeure;
        var minutes = pixels * minutesParPixel;
        return (int)(Math.Round(minutes / 15.0) * 15);
    }

    private void Poignee_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _decalageHautCumule = 0;
        _decalageBasCumule = 0;

        if (ViewModel is not null)
        {
            RaiseEvent(new RoutedEventArgs(RedimensionneEvent, this));
        }
    }
}
