using System.Windows;
using Wpf.Ui.Controls;

namespace GestPlan.App;

/// <summary>
/// Fenêtre de saisie du nombre de semaines vers lesquelles dupliquer la semaine affichée.
/// </summary>
public partial class DupliquerSemaineWindow : FluentWindow
{
    public int NombreDeSemaines { get; private set; }

    public DupliquerSemaineWindow()
    {
        InitializeComponent();
    }

    private void Dupliquer_Click(object sender, RoutedEventArgs e)
    {
        NombreDeSemaines = (int)(SaisieNombreSemaines.Value ?? 1);
        DialogResult = true;
        Close();
    }
}
