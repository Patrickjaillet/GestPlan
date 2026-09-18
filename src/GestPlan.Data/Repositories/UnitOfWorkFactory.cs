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

        public IRepository<Core.Entites.EmployePoste> EmployesPostes => interieur.EmployesPostes;

        public IRepository<Core.Entites.Indisponibilite> Indisponibilites => interieur.Indisponibilites;

        public IRepository<Core.Entites.CreneauPlanning> CreneauxPlanning => interieur.CreneauxPlanning;

        public IRepository<Core.Entites.TypeAbsence> TypesAbsence => interieur.TypesAbsence;

        public IRepository<Core.Entites.Absence> Absences => interieur.Absences;

        public IRepository<Core.Entites.ReglesAcquisitionConges> ReglesAcquisitionConges => interieur.ReglesAcquisitionConges;

        public IRepository<Core.Entites.ReglesConformite> ReglesConformite => interieur.ReglesConformite;

        public IRepository<Core.Entites.JournalAudit> JournauxAudit => interieur.JournauxAudit;

        public Task<int> EnregistrerAsync(CancellationToken cancellationToken = default) =>
            interieur.EnregistrerAsync(cancellationToken);

        public void Dispose() => scope.Dispose();
    }
}
