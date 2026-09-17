using GestPlan.App.ViewModels;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class EmployesPage : Page
{
    public EmployesPage(EmployesViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
