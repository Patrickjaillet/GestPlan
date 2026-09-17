using GestPlan.App.ViewModels;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class UtilisateursPage : Page
{
    public UtilisateursPage(UtilisateursViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
