using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GestPlan.App.Services;
using GestPlan.App.ViewModels;
using GestPlan.App.Views.Controls;
using GestPlan.Core.Enumerations;

namespace GestPlan.App.Views.Pages;

public partial class PlanningPage : Page
{
    private const double LargeurColonne = 150;
    private const double LargeurLibelle = 60;
    private const int HeureFinGrille = 22;

    private readonly PlanningViewModel _viewModel;

    public PlanningPage(PlanningViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();

        viewModel.JoursAffiches.CollectionChanged += (_, _) => ReconstruireSections();
        viewModel.Creneaux.CollectionChanged += Creneaux_CollectionChanged;

        Loaded += (_, _) => ReconstruireSections();
    }

    private void Creneaux_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => ReconstruireSections();

    /// <summary>
    /// Reconstruit l'affichage : une seule grille en mode normal, ou une grille par site
    /// (avec son nom en en-tête) en vue consolidée multi-site, pour permettre la comparaison.
    /// </summary>
    private void ReconstruireSections()
    {
        ConteneurSections.Children.Clear();

        if (_viewModel.JoursAffiches.Count == 0)
        {
            return;
        }

        if (!_viewModel.AfficherVueConsolidee)
        {
            ConteneurSections.Children.Add(ConstruireGrilleSite(_viewModel.Creneaux));
            return;
        }

        foreach (var groupe in _viewModel.Creneaux.GroupBy(c => c.NomSite).OrderBy(g => g.Key))
        {
            var titre = new TextBlock
            {
                Text = string.IsNullOrEmpty(groupe.Key) ? "Sans site" : groupe.Key,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Margin = new Thickness(0, 16, 0, 8)
            };
            ConteneurSections.Children.Add(titre);
            ConteneurSections.Children.Add(ConstruireGrilleSite(groupe));
        }
    }

    private Grid ConstruireGrilleSite(IEnumerable<CreneauPlanningViewModel> creneauxSite)
    {
        return _viewModel.VueSelectionnee switch
        {
            TypeVuePlanning.ParEmploye => ConstruireGrilleParEntite(creneauxSite, c => c.EmployeId),
            TypeVuePlanning.ParPoste => ConstruireGrilleParEntite(creneauxSite, c => c.PosteId),
            _ => ConstruireGrilleParJour(creneauxSite)
        };
    }

    private Grid ConstruireGrilleParJour(IEnumerable<CreneauPlanningViewModel> creneauxSite)
    {
        var grille = new Grid();
        var hauteurGrille = (HeureFinGrille - HeureVersPositionConverter.HeureDebutGrille) * HeureVersPositionConverter.PixelsParHeure;

        grille.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(LargeurLibelle) });
        AjouterLibellesHeures(grille, hauteurGrille);

        var canevasParJour = new Dictionary<DateOnly, Canvas>();

        for (var i = 0; i < _viewModel.JoursAffiches.Count; i++)
        {
            var jour = _viewModel.JoursAffiches[i];
            grille.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(LargeurColonne) });

            var titre = jour.ToString("dddd dd/MM", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
            var colonne = ConstruireColonne(titre, hauteurGrille, jour, out var canevas);
            Grid.SetColumn(colonne, i + 1);
            grille.Children.Add(colonne);
            canevasParJour[jour] = canevas;
        }

        foreach (var creneau in creneauxSite)
        {
            if (canevasParJour.TryGetValue(creneau.Date, out var canevas))
            {
                AjouterCreneauSurGrille(creneau, canevas);
            }
        }

        return grille;
    }

    /// <summary>
    /// Construit une grille dont les colonnes sont des entités (employés ou postes) plutôt que
    /// des jours : une seule journée est affichée (<see cref="PlanningViewModel.DateReference"/>),
    /// ce qui permet de voir en un coup d'œil qui/quoi est occupé sur cette journée.
    /// </summary>
    private Grid ConstruireGrilleParEntite(IEnumerable<CreneauPlanningViewModel> creneauxSite, Func<CreneauPlanningViewModel, int> selecteurEntite)
    {
        var grille = new Grid();
        var hauteurGrille = (HeureFinGrille - HeureVersPositionConverter.HeureDebutGrille) * HeureVersPositionConverter.PixelsParHeure;
        var jourUnique = _viewModel.JoursAffiches.Count > 0 ? _viewModel.JoursAffiches[0] : _viewModel.DateReference;

        grille.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(LargeurLibelle) });
        AjouterLibellesHeures(grille, hauteurGrille);

        var canevasParEntite = new Dictionary<int, Canvas>();

        for (var i = 0; i < _viewModel.ColonnesParEntite.Count; i++)
        {
            var entite = _viewModel.ColonnesParEntite[i];
            grille.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(LargeurColonne) });

            var colonne = ConstruireColonne(entite.Libelle, hauteurGrille, jourUnique, out var canevas);
            Grid.SetColumn(colonne, i + 1);
            grille.Children.Add(colonne);
            canevasParEntite[entite.Id] = canevas;
        }

        foreach (var creneau in creneauxSite)
        {
            if (canevasParEntite.TryGetValue(selecteurEntite(creneau), out var canevas))
            {
                AjouterCreneauSurGrille(creneau, canevas);
            }
        }

        return grille;
    }

    private static void AjouterLibellesHeures(Grid grille, double hauteurGrille)
    {
        var canevas = new Canvas { Width = LargeurLibelle, Height = hauteurGrille };
        Grid.SetColumn(canevas, 0);

        for (var heure = HeureVersPositionConverter.HeureDebutGrille; heure <= HeureFinGrille; heure++)
        {
            var libelle = new TextBlock
            {
                Text = $"{heure:00}:00",
                Opacity = 0.6,
                FontSize = 11
            };
            Canvas.SetTop(libelle, (heure - HeureVersPositionConverter.HeureDebutGrille) * HeureVersPositionConverter.PixelsParHeure);
            Canvas.SetLeft(libelle, 4);
            canevas.Children.Add(libelle);
        }

        grille.Children.Add(canevas);
    }

    /// <summary>
    /// Construit une colonne de grille avec son titre. Le <paramref name="jourPourCreation"/> est
    /// toujours le jour effectivement associé à cette colonne (utile pour le double-clic de création
    /// et le glisser-déposer, même en vue « Par employé »/« Par poste » où une seule journée est affichée).
    /// </summary>
    private Border ConstruireColonne(string titre, double hauteurGrille, DateOnly jourPourCreation, out Canvas canevas)
    {
        canevas = new Canvas
        {
            Width = LargeurColonne,
            Height = hauteurGrille,
            Background = Brushes.Transparent,
            AllowDrop = true,
            Tag = jourPourCreation
        };

        canevas.MouseLeftButtonDown += Canevas_MouseLeftButtonDown;
        canevas.Drop += Canevas_Drop;
        canevas.DragOver += (_, e) => e.Effects = DragDropEffects.Move;

        for (var heure = HeureVersPositionConverter.HeureDebutGrille; heure < HeureFinGrille; heure++)
        {
            var ligne = new System.Windows.Shapes.Line
            {
                X1 = 0,
                X2 = LargeurColonne,
                Y1 = (heure - HeureVersPositionConverter.HeureDebutGrille) * HeureVersPositionConverter.PixelsParHeure,
                Y2 = (heure - HeureVersPositionConverter.HeureDebutGrille) * HeureVersPositionConverter.PixelsParHeure,
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.5
            };
            canevas.Children.Add(ligne);
        }

        var entete = new StackPanel();
        var titreColonne = new TextBlock
        {
            Text = titre,
            FontWeight = FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
            Margin = new Thickness(0, 0, 0, 4)
        };
        entete.Children.Add(titreColonne);

        var conteneur = new StackPanel();
        conteneur.Children.Add(entete);
        conteneur.Children.Add(canevas);

        return new Border { Child = conteneur, BorderThickness = new Thickness(0, 0, 1, 0), BorderBrush = Brushes.LightGray };
    }

    private void AjouterCreneauSurGrille(CreneauPlanningViewModel creneau, Canvas canevas)
    {
        var controle = new CreneauControl { DataContext = creneau };
        controle.Redimensionne += async (_, _) => await _viewModel.RedimensionnerCreneauCommand.ExecuteAsync(
            new RedimensionnementParametres(creneau.Id, creneau.HeureDebut, creneau.HeureFin));
        controle.MouseRightButtonUp += (_, e) => { AfficherMenuCreneau(controle, creneau); e.Handled = true; };

        Canvas.SetTop(controle, (creneau.HeureDebut.ToTimeSpan().TotalHours - HeureVersPositionConverter.HeureDebutGrille) * HeureVersPositionConverter.PixelsParHeure);
        Canvas.SetLeft(controle, 4);
        controle.Height = Math.Max((creneau.HeureFin.ToTimeSpan() - creneau.HeureDebut.ToTimeSpan()).TotalHours * HeureVersPositionConverter.PixelsParHeure, 16);

        canevas.Children.Add(controle);
    }

    private void AfficherMenuCreneau(FrameworkElement element, CreneauPlanningViewModel creneau)
    {
        var menu = new ContextMenu();

        foreach (var statut in Enum.GetValues<StatutCreneau>())
        {
            var item = new MenuItem { Header = statut.ToString() };
            item.Click += async (_, _) => await _viewModel.ChangerStatutCommand.ExecuteAsync((creneau, statut));
            menu.Items.Add(item);
        }

        menu.Items.Add(new Separator());

        var supprimer = new MenuItem { Header = "Supprimer" };
        supprimer.Click += async (_, _) => await _viewModel.SupprimerCreneauCommand.ExecuteAsync(creneau);
        menu.Items.Add(supprimer);

        element.ContextMenu = menu;
        menu.IsOpen = true;
    }

    private void Canevas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not Canvas canevas || canevas.Tag is not DateOnly jour)
        {
            return;
        }

        if (e.ClickCount != 2)
        {
            return;
        }

        var position = e.GetPosition(canevas);
        var heure = HeureVersPositionConverter.HeureDebutGrille + position.Y / HeureVersPositionConverter.PixelsParHeure;
        var heureArrondie = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(Math.Round(heure * 4) * 15));

        OuvrirFormulaireNouveauCreneau(jour, heureArrondie);
    }

    private void Canevas_Drop(object sender, DragEventArgs e)
    {
        if (sender is not Canvas canevas || canevas.Tag is not DateOnly jour)
        {
            return;
        }

        if (e.Data.GetData(typeof(CreneauPlanningViewModel)) is not CreneauPlanningViewModel creneauSource)
        {
            return;
        }

        var position = e.GetPosition(canevas);
        var heure = HeureVersPositionConverter.HeureDebutGrille + position.Y / HeureVersPositionConverter.PixelsParHeure;
        var nouvelleHeureDebut = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(Math.Round(heure * 4) * 15));
        var duree = creneauSource.HeureFin.ToTimeSpan() - creneauSource.HeureDebut.ToTimeSpan();
        var nouvelleHeureFin = TimeOnly.FromTimeSpan(nouvelleHeureDebut.ToTimeSpan() + duree);

        creneauSource.Date = jour;
        creneauSource.HeureDebut = nouvelleHeureDebut;
        creneauSource.HeureFin = nouvelleHeureFin;

        _ = _viewModel.RedimensionnerCreneauCommand.ExecuteAsync(
            new RedimensionnementParametres(creneauSource.Id, nouvelleHeureDebut, nouvelleHeureFin));
    }

    private void OuvrirFormulaireNouveauCreneau(DateOnly jour, TimeOnly heureDebut)
    {
        var fenetre = new NouveauCreneauWindow(_viewModel, jour, heureDebut) { Owner = Window.GetWindow(this) };
        fenetre.ShowDialog();
    }

    private async void DupliquerSemaine_Click(object sender, RoutedEventArgs e)
    {
        var fenetre = new DupliquerSemaineWindow { Owner = Window.GetWindow(this) };
        if (fenetre.ShowDialog() == true)
        {
            await _viewModel.DupliquerSemaineCommand.ExecuteAsync(fenetre.NombreDeSemaines);
        }
    }
}
