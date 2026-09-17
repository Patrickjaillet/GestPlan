using GestPlan.App.Services;
using GestPlan.App.ViewModels;
using GestPlan.App.Views.Pages;
using GestPlan.Core.Enumerations;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GestPlan.App;

/// <summary>
/// Fenêtre principale de l'application, hébergeant la navigation entre modules.
/// </summary>
public partial class MainWindow : FluentWindow
{
    public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService, ISessionUtilisateurService session)
    {
        DataContext = viewModel;
        InitializeComponent();

        SystemThemeWatcher.Watch(this);

        navigationService.SetNavigationControl(NavigationVuePrincipale);

        ElementUtilisateurs.Visibility = session.APourRoleMinimum(RoleUtilisateur.Admin)
            ? System.Windows.Visibility.Visible
            : System.Windows.Visibility.Collapsed;

        Loaded += (_, _) => navigationService.Navigate(typeof(PlanningPage));
    }
}
