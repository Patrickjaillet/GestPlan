namespace GestPlan.App.Services;

/// <inheritdoc cref="IServicePileAnnulation"/>
public class ServicePileAnnulation : IServicePileAnnulation
{
    private sealed record Action(Func<Task> Annuler, Func<Task> Retablir);

    private readonly Stack<Action> _pileAnnulation = new();
    private readonly Stack<Action> _pileRetablissement = new();

    public bool PeutAnnuler => _pileAnnulation.Count > 0;

    public bool PeutRetablir => _pileRetablissement.Count > 0;

    public event EventHandler? Change;

    public void Empiler(Func<Task> annuler, Func<Task> retablir)
    {
        _pileAnnulation.Push(new Action(annuler, retablir));
        _pileRetablissement.Clear();
        Change?.Invoke(this, EventArgs.Empty);
    }

    public async Task AnnulerAsync()
    {
        if (_pileAnnulation.Count == 0)
        {
            return;
        }

        var action = _pileAnnulation.Pop();
        await action.Annuler();
        _pileRetablissement.Push(action);
        Change?.Invoke(this, EventArgs.Empty);
    }

    public async Task RetablirAsync()
    {
        if (_pileRetablissement.Count == 0)
        {
            return;
        }

        var action = _pileRetablissement.Pop();
        await action.Retablir();
        _pileAnnulation.Push(action);
        Change?.Invoke(this, EventArgs.Empty);
    }

    public void Vider()
    {
        _pileAnnulation.Clear();
        _pileRetablissement.Clear();
        Change?.Invoke(this, EventArgs.Empty);
    }
}
