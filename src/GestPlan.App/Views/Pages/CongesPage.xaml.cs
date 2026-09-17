using GestPlan.App.ViewModels;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class CongesPage : Page
{
    public CongesPage(CongesViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
