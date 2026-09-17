using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestPlan.Data.Import;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Fenêtre d'import en masse d'employés depuis un fichier CSV ou Excel.
/// </summary>
public partial class ImportEmployesViewModel : ObservableObject
{
    private readonly IServiceImportEmployes _serviceImport;

    [ObservableProperty]
    private string? _cheminFichier;

    [ObservableProperty]
    private string? _messageResultat;

    [ObservableProperty]
    private bool _importEnCours;

    public ObservableCollection<LigneImportEmploye> LignesAnalysees { get; } = [];

    public bool AuMoinsUneLigneValide => LignesAnalysees.Any(l => l.EstValide);

    public event EventHandler? ImportTermine;

    public ImportEmployesViewModel(IServiceImportEmployes serviceImport)
    {
        _serviceImport = serviceImport;
    }

    public void AnalyserFichier(string cheminFichier)
    {
        CheminFichier = cheminFichier;
        MessageResultat = null;
        LignesAnalysees.Clear();

        using var flux = File.OpenRead(cheminFichier);

        var lignes = Path.GetExtension(cheminFichier).Equals(".csv", StringComparison.OrdinalIgnoreCase)
            ? _serviceImport.LireEtValiderCsv(flux)
            : _serviceImport.LireEtValiderExcel(flux);

        foreach (var ligne in lignes)
        {
            LignesAnalysees.Add(ligne);
        }

        OnPropertyChanged(nameof(AuMoinsUneLigneValide));
    }

    [RelayCommand]
    private async Task ImporterAsync()
    {
        ImportEnCours = true;
        try
        {
            var nombreImporte = await _serviceImport.ImporterAsync(LignesAnalysees.Where(l => l.EstValide));
            MessageResultat = $"{nombreImporte} employé(s) importé(s) avec succès.";
            ImportTermine?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            ImportEnCours = false;
        }
    }
}
