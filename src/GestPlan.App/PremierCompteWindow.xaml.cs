using GestPlan.App.ViewModels;
using Wpf.Ui.Controls;

namespace GestPlan.App;

/// <summary>
/// Fenêtre de création du tout premier compte administrateur, affichée à la place
/// de <see cref="ConnexionWindow"/> lorsqu'aucun utilisateur n'existe encore.
/// </summary>
public partial class PremierCompteWindow : FluentWindow
{
    public PremierCompteWindow(PremierCompteViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();

        viewModel.CompteCree += (_, _) =>
        {
            DialogResult = true;
            Close();
        };
    }
}
