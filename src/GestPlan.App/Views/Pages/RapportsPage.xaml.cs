using GestPlan.App.ViewModels;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class RapportsPage : Page
{
    public RapportsPage(RapportsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
