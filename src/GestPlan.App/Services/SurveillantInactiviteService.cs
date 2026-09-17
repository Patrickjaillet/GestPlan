using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GestPlan.App.Services;

/// <summary>
/// Déconnecte automatiquement l'utilisateur après une durée configurable d'inactivité
/// (absence de mouvement de souris, clic ou saisie clavier).
/// </summary>
public class SurveillantInactiviteService : IDisposable
{
    private readonly ISessionUtilisateurService _session;
    private readonly DispatcherTimer _minuteur;
    private TimeSpan _delaiInactivite = TimeSpan.FromMinutes(15);

    public SurveillantInactiviteService(ISessionUtilisateurService session)
    {
        _session = session;

        _minuteur = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _minuteur.Tick += (_, _) => VerifierInactivite();

        Application.Current.Dispatcher.Invoke(() =>
        {
            InputManager.Current.PreProcessInput += (_, _) => Reinitialiser();
        });

        Reinitialiser();
    }

    public TimeSpan DelaiInactivite
    {
        get => _delaiInactivite;
        set => _delaiInactivite = value;
    }

    private DateTime _derniereActivite = DateTime.UtcNow;

    public void Demarrer() => _minuteur.Start();

    public void Arreter() => _minuteur.Stop();

    private void Reinitialiser() => _derniereActivite = DateTime.UtcNow;

    private void VerifierInactivite()
    {
        if (!_session.EstConnecte)
        {
            return;
        }

        if (DateTime.UtcNow - _derniereActivite >= DelaiInactivite)
        {
            _session.FermerSession();
        }
    }

    public void Dispose() => _minuteur.Stop();
}
