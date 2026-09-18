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
        EmployesPostes = new Repository<EmployePoste>(contexte);
        Indisponibilites = new Repository<Indisponibilite>(contexte);
        CreneauxPlanning = new Repository<CreneauPlanning>(contexte);
        TypesAbsence = new Repository<TypeAbsence>(contexte);
        Absences = new Repository<Absence>(contexte);
        ReglesAcquisitionConges = new Repository<ReglesAcquisitionConges>(contexte);
        ReglesConformite = new Repository<ReglesConformite>(contexte);
        JournauxAudit = new Repository<JournalAudit>(contexte);
    }

    public IRepository<Site> Sites { get; }

    public IRepository<Utilisateur> Utilisateurs { get; }

    public IRepository<Employe> Employes { get; }

    public IRepository<Poste> Postes { get; }

    public IRepository<Contrat> Contrats { get; }

    public IRepository<EmployePoste> EmployesPostes { get; }

    public IRepository<Indisponibilite> Indisponibilites { get; }

    public IRepository<CreneauPlanning> CreneauxPlanning { get; }

    public IRepository<TypeAbsence> TypesAbsence { get; }

    public IRepository<Absence> Absences { get; }

    public IRepository<ReglesAcquisitionConges> ReglesAcquisitionConges { get; }

    public IRepository<ReglesConformite> ReglesConformite { get; }

    public IRepository<JournalAudit> JournauxAudit { get; }

    public Task<int> EnregistrerAsync(CancellationToken cancellationToken = default) =>
        _contexte.SaveChangesAsync(cancellationToken);

    public void Dispose() => _contexte.Dispose();
}
