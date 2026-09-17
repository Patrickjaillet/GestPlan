using GestPlan.App.Services;
using GestPlan.App.ViewModels;
using GestPlan.Core.Entites;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Windows.Controls;

namespace GestPlan.App.Views.Pages;

public partial class EmployesPage : Page
{
    private readonly EmployesViewModel _viewModel;
    private readonly IServicePhotosEmployes _servicePhotos;

    public EmployesPage(EmployesViewModel viewModel, IServicePhotosEmployes servicePhotos)
    {
        _viewModel = viewModel;
        _servicePhotos = servicePhotos;
        DataContext = viewModel;
        InitializeComponent();
    }

    private async void ListeEmployes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListView { SelectedItem: Employe employe })
        {
            await _viewModel.SelectionnerEmployeCommand.ExecuteAsync(employe);
        }
    }

    private async void Importer_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var importWindow = App.Services.GetRequiredService<ImportEmployesWindow>();
        if (importWindow.ShowDialog() == true)
        {
            await _viewModel.RechargerCommand.ExecuteAsync(null);
        }
    }

    private void ChoisirPhoto_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_viewModel.EmployeSelectionne is null)
        {
            return;
        }

        var dialogue = new OpenFileDialog
        {
            Filter = "Images (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
        };

        if (dialogue.ShowDialog() == true)
        {
            _viewModel.EmployeSelectionne.CheminPhoto = _servicePhotos.EnregistrerPhoto(dialogue.FileName);
        }
    }
}
