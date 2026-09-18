namespace GestPlan.App.Services;

/// <summary>
/// Pile d'actions annulables/rétablissables en session (undo/redo), pour les modifications
/// apportées au planning. Ne persiste rien entre sessions : la pile est vidée à la fermeture.
/// </summary>
public interface IServicePileAnnulation
{
    bool PeutAnnuler { get; }

    bool PeutRetablir { get; }

    event EventHandler? Change;

    /// <summary>
    /// Empile une action déjà exécutée, avec sa fonction d'annulation et de rétablissement.
    /// Vide la pile de rétablissement (toute nouvelle action invalide les rétablissements en attente).
    /// </summary>
    void Empiler(Func<Task> annuler, Func<Task> retablir);

    Task AnnulerAsync();

    Task RetablirAsync();

    void Vider();
}
