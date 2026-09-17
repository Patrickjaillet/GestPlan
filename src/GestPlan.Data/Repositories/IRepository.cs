using System.Linq.Expressions;

namespace GestPlan.Data.Repositories;

/// <summary>
/// Accès générique en lecture/écriture à un type d'entité, sans exposer EF Core aux couches supérieures.
/// </summary>
public interface IRepository<TEntite> where TEntite : class
{
    Task<TEntite?> ObtenirParIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TEntite?> ObtenirUnAsync(Expression<Func<TEntite, bool>> predicat, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntite>> ObtenirTousAsync(CancellationToken cancellationToken = default);

    Task AjouterAsync(TEntite entite, CancellationToken cancellationToken = default);

    void Modifier(TEntite entite);

    void Supprimer(TEntite entite);
}
