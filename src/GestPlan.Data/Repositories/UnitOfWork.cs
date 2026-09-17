using GestPlan.Core.Entites;

namespace GestPlan.Data.Repositories;

/// <summary>
/// Implémentation par défaut de <see cref="IUnitOfWork"/>, adossée à un unique <see cref="GestPlanDbContext"/>.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly GestPlanDbContext _contexte;

    public UnitOfWork(GestPlanDbContext contexte)
    {
        _contexte = contexte;
        Sites = new Repository<Site>(contexte);
        Utilisateurs = new Repository<Utilisateur>(contexte);
        Employes = new Repository<Employe>(contexte);
        Postes = new Repository<Poste>(contexte);
        Contrats = new Repository<Contrat>(contexte);
    }

    public IRepository<Site> Sites { get; }

    public IRepository<Utilisateur> Utilisateurs { get; }

    public IRepository<Employe> Employes { get; }

    public IRepository<Poste> Postes { get; }

    public IRepository<Contrat> Contrats { get; }

    public Task<int> EnregistrerAsync(CancellationToken cancellationToken = default) =>
        _contexte.SaveChangesAsync(cancellationToken);
}
