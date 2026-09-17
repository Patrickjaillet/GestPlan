using GestPlan.App.ViewModels;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class PlanningPage : Page
{
    public PlanningPage(PlanningViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
