using GestPlan.App.ViewModels;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class ParametresPage : Page
{
    public ParametresPage(ParametresViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
