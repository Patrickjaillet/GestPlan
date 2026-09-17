using GestPlan.Core.Entites;

namespace GestPlan.Data.Repositories;

/// <summary>
/// Regroupe les repositories du domaine et coordonne la persistance des changements
/// dans une unique transaction implicite via <see cref="GestPlanDbContext.SaveChangesAsync"/>.
/// </summary>
public interface IUnitOfWork
{
    IRepository<Site> Sites { get; }

    IRepository<Utilisateur> Utilisateurs { get; }

    IRepository<Employe> Employes { get; }

    IRepository<Poste> Postes { get; }

    IRepository<Contrat> Contrats { get; }

    Task<int> EnregistrerAsync(CancellationToken cancellationToken = default);
}
