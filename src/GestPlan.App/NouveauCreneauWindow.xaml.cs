using System.Windows;
using GestPlan.App.ViewModels;
using GestPlan.Core.Entites;
using Wpf.Ui.Controls;

namespace GestPlan.App;

/// <summary>
/// Fenêtre de création rapide d'un créneau de planning, ouverte par double-clic
/// sur une cellule vide de la grille.
/// </summary>
public partial class NouveauCreneauWindow : FluentWindow
{
    private readonly PlanningViewModel _viewModel;
    private readonly DateOnly _jour;

    public NouveauCreneauWindow(PlanningViewModel viewModel, DateOnly jour, TimeOnly heureDebut)
    {
        _viewModel = viewModel;
        _jour = jour;
        InitializeComponent();

        ListeEmployes.ItemsSource = viewModel.Employes;
        ListePostes.ItemsSource = viewModel.Postes;

        SelecteurHeureDebut.SelectedTime = heureDebut.ToTimeSpan();
        SelecteurHeureFin.SelectedTime = heureDebut.AddHours(4).ToTimeSpan();
    }

    private async void Creer_Click(object sender, RoutedEventArgs e)
    {
        if (ListeEmployes.SelectedItem is not Employe employe ||
            ListePostes.SelectedItem is not Poste poste ||
            SelecteurHeureDebut.SelectedTime is not TimeSpan heureDebut ||
            SelecteurHeureFin.SelectedTime is not TimeSpan heureFin)
        {
            MessageErreurTexte.Text = "Veuillez renseigner tous les champs.";
            MessageErreurTexte.Visibility = Visibility.Visible;
            return;
        }

        var parametres = new NouveauCreneauParametres(
            employe.Id,
            poste.SiteId,
            poste.Id,
            _jour,
            TimeOnly.FromTimeSpan(heureDebut),
            TimeOnly.FromTimeSpan(heureFin));

        await _viewModel.CreerCreneauCommand.ExecuteAsync(parametres);

        if (string.IsNullOrEmpty(_viewModel.MessageErreur))
        {
            Close();
        }
        else
        {
            MessageErreurTexte.Text = _viewModel.MessageErreur;
            MessageErreurTexte.Visibility = Visibility.Visible;
        }
    }
}
