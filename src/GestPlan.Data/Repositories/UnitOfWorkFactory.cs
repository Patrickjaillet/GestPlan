using Microsoft.Extensions.DependencyInjection;

namespace GestPlan.Data.Repositories;

/// <inheritdoc cref="IUnitOfWorkFactory"/>
public class UnitOfWorkFactory(IServiceScopeFactory scopeFactory) : IUnitOfWorkFactory
{
    public IUnitOfWork Creer()
    {
        var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        return new UnitOfWorkAvecScope(unitOfWork, scope);
    }

    private sealed class UnitOfWorkAvecScope(IUnitOfWork interieur, IServiceScope scope) : IUnitOfWork
    {
        public IRepository<Core.Entites.Site> Sites => interieur.Sites;

        public IRepository<Core.Entites.Utilisateur> Utilisateurs => interieur.Utilisateurs;

        public IRepository<Core.Entites.Employe> Employes => interieur.Employes;

        public IRepository<Core.Entites.Poste> Postes => interieur.Postes;

        public IRepository<Core.Entites.Contrat> Contrats => interieur.Contrats;

        public IRepository<Core.Entites.ReglesConformite> ReglesConformite => interieur.ReglesConformite;

        public Task<int> EnregistrerAsync(CancellationToken cancellationToken = default) =>
            interieur.EnregistrerAsync(cancellationToken);

        public void Dispose() => scope.Dispose();
    }
}
