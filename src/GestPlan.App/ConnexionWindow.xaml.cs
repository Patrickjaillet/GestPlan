using GestPlan.App.ViewModels;
using Wpf.Ui.Controls;

namespace GestPlan.App;

/// <summary>
/// Fenêtre de connexion affichée au démarrage de l'application, avant <see cref="MainWindow"/>.
/// </summary>
public partial class ConnexionWindow : FluentWindow
{
    public ConnexionWindow(ConnexionViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();

        viewModel.ConnexionReussie += (_, _) =>
        {
            DialogResult = true;
            Close();
        };
    }
}
