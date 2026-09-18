using System.IO;
using System.Windows;
using System.Windows.Controls;
using GestPlan.App.ViewModels;
using Microsoft.Win32;

namespace GestPlan.App.Views.Pages;

public partial class RapportsPage : Page
{
    private readonly RapportsViewModel _viewModel;

    public RapportsPage(RapportsViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }

    private void ExporterPdf_Click(object sender, RoutedEventArgs e)
    {
        var pdf = _viewModel.GenererExportPdf();
        if (pdf is null)
        {
            return;
        }

        var dialogue = new SaveFileDialog
        {
            Filter = "Fichier PDF (*.pdf)|*.pdf",
            FileName = "rapport-conformite.pdf"
        };

        if (dialogue.ShowDialog() == true)
        {
            File.WriteAllBytes(dialogue.FileName, pdf);
            System.Windows.MessageBox.Show(
                "Le rapport de conformité a été exporté avec succès.",
                "Export réussi",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
    }
}
