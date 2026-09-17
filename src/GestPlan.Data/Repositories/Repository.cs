using Microsoft.EntityFrameworkCore;

namespace GestPlan.Data.Repositories;

/// <summary>
/// Implémentation EF Core générique de <see cref="IRepository{TEntite}"/>.
/// Les modifications ne sont persistées qu'après appel à <see cref="IUnitOfWork.EnregistrerAsync"/>.
/// </summary>
public class Repository<TEntite>(GestPlanDbContext contexte) : IRepository<TEntite> where TEntite : class
{
    private readonly DbSet<TEntite> _ensemble = contexte.Set<TEntite>();

    public async Task<TEntite?> ObtenirParIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _ensemble.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<TEntite>> ObtenirTousAsync(CancellationToken cancellationToken = default) =>
        await _ensemble.ToListAsync(cancellationToken);

    public async Task AjouterAsync(TEntite entite, CancellationToken cancellationToken = default) =>
        await _ensemble.AddAsync(entite, cancellationToken);

    public void Modifier(TEntite entite) => _ensemble.Update(entite);

    public void Supprimer(TEntite entite) => _ensemble.Remove(entite);
}
