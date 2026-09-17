using System.Windows;
using GestPlan.App.ViewModels;
using Microsoft.Win32;
using Wpf.Ui.Controls;

namespace GestPlan.App;

/// <summary>
/// Fenêtre d'import en masse d'employés depuis un fichier CSV ou Excel.
/// </summary>
public partial class ImportEmployesWindow : FluentWindow
{
    private readonly ImportEmployesViewModel _viewModel;

    public ImportEmployesWindow(ImportEmployesViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();

        viewModel.ImportTermine += (_, _) => DialogResult = true;
    }

    private void ChoisirFichier_Click(object sender, RoutedEventArgs e)
    {
        var dialogue = new OpenFileDialog
        {
            Filter = "Fichiers CSV et Excel (*.csv;*.xlsx)|*.csv;*.xlsx|Tous les fichiers (*.*)|*.*"
        };

        if (dialogue.ShowDialog() == true)
        {
            _viewModel.AnalyserFichier(dialogue.FileName);
        }
    }
}
