using GestPlan.App.ViewModels;
using GestPlan.App.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GestPlan.App;

/// <summary>
/// Fenêtre principale de l'application, hébergeant la navigation entre modules.
/// </summary>
public partial class MainWindow : FluentWindow
{
    public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService)
    {
        DataContext = viewModel;
        InitializeComponent();

        SystemThemeWatcher.Watch(this);

        navigationService.SetNavigationControl(NavigationVuePrincipale);

        Loaded += (_, _) => navigationService.Navigate(typeof(PlanningPage));
    }
}
