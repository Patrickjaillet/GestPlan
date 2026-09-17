using GestPlan.App.ViewModels;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class AProposPage : Page
{
    public AProposPage(AProposViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
