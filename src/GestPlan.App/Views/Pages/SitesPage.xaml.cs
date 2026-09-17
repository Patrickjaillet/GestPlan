using GestPlan.App.ViewModels;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class SitesPage : Page
{
    public SitesPage(SitesViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
