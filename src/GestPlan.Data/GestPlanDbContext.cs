using System.Text.Encodings.Web;
using System.Text.Json;
using GestPlan.Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace GestPlan.Data;

/// <summary>
/// Contexte EF Core de l'application, basé sur SQLite. La piste d'audit
/// (<see cref="JournalAudit"/>) est alimentée automatiquement à chaque
/// <see cref="SaveChangesAsync"/> pour toute entité modifiée hors audit lui-même.
/// </summary>
public class GestPlanDbContext(DbContextOptions<GestPlanDbContext> options) : DbContext(options)
{
    public int? UtilisateurCourantId { get; set; }

    public DbSet<Site> Sites => Set<Site>();

    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();

    public DbSet<Employe> Employes => Set<Employe>();

    public DbSet<Poste> Postes => Set<Poste>();

    public DbSet<Contrat> Contrats => Set<Contrat>();

    public DbSet<EmployeSite> EmployesSites => Set<EmployeSite>();

    public DbSet<EmployePoste> EmployesPostes => Set<EmployePoste>();

    public DbSet<Indisponibilite> Indisponibilites => Set<Indisponibilite>();

    public DbSet<ReglesConformite> ReglesConformite => Set<ReglesConformite>();

    public DbSet<CreneauPlanning> CreneauxPlanning => Set<CreneauPlanning>();

    public DbSet<JournalAudit> JournauxAudit => Set<JournalAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Site>(entite =>
        {
            entite.HasIndex(s => s.Nom).IsUnique();
        });

        modelBuilder.Entity<Utilisateur>(entite =>
        {
            entite.HasIndex(u => u.NomUtilisateur).IsUnique();
            entite.HasOne(u => u.SiteAssigne)
                .WithMany()
                .HasForeignKey(u => u.SiteAssigneId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Poste>(entite =>
        {
            entite.HasOne(p => p.Site)
                .WithMany(s => s.Postes)
                .HasForeignKey(p => p.SiteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employe>(entite =>
        {
            entite.HasOne(e => e.SitePrincipal)
                .WithMany(s => s.Employes)
                .HasForeignKey(e => e.SitePrincipalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeSite>(entite =>
        {
            entite.HasKey(es => new { es.EmployeId, es.SiteId });

            entite.HasOne(es => es.Employe)
                .WithMany(e => e.SitesSecondaires)
                .HasForeignKey(es => es.EmployeId)
                .OnDelete(DeleteBehavior.Cascade);

            entite.HasOne(es => es.Site)
                .WithMany(s => s.SitesSecondaires)
                .HasForeignKey(es => es.SiteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReglesConformite>(entite =>
        {
            entite.HasIndex(r => r.SiteId).IsUnique();

            entite.HasOne(r => r.Site)
                .WithOne(s => s.ReglesConformite)
                .HasForeignKey<ReglesConformite>(r => r.SiteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Contrat>(entite =>
        {
            entite.Property(c => c.TauxHoraireBrut).HasPrecision(10, 2);
            entite.Property(c => c.HeuresHebdomadaires).HasPrecision(5, 2);

            entite.HasOne(c => c.Employe)
                .WithMany(e => e.Contrats)
                .HasForeignKey(c => c.EmployeId)
                .OnDelete(DeleteBehavior.Cascade);

            entite.HasOne(c => c.Site)
                .WithMany(s => s.Contrats)
                .HasForeignKey(c => c.SiteId)
                .OnDelete(DeleteBehavior.Restrict);

            entite.HasOne(c => c.Poste)
                .WithMany(p => p.Contrats)
                .HasForeignKey(c => c.PosteId)
                .OnDelete(DeleteBehavior.Restrict);

            entite.HasOne(c => c.ContratPrecedent)
                .WithMany()
                .HasForeignKey(c => c.ContratPrecedentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployePoste>(entite =>
        {
            entite.HasKey(ep => new { ep.EmployeId, ep.PosteId });

            entite.HasOne(ep => ep.Employe)
                .WithMany(e => e.PostesAutorises)
                .HasForeignKey(ep => ep.EmployeId)
                .OnDelete(DeleteBehavior.Cascade);

            entite.HasOne(ep => ep.Poste)
                .WithMany(p => p.EmployesAutorises)
                .HasForeignKey(ep => ep.PosteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Indisponibilite>(entite =>
        {
            entite.HasOne(i => i.Employe)
                .WithMany(e => e.Indisponibilites)
                .HasForeignKey(i => i.EmployeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CreneauPlanning>(entite =>
        {
            entite.HasIndex(c => new { c.EmployeId, c.Date });

            entite.HasOne(c => c.Employe)
                .WithMany(e => e.CreneauxPlanning)
                .HasForeignKey(c => c.EmployeId)
                .OnDelete(DeleteBehavior.Cascade);

            entite.HasOne(c => c.Site)
                .WithMany()
                .HasForeignKey(c => c.SiteId)
                .OnDelete(DeleteBehavior.Restrict);

            entite.HasOne(c => c.Poste)
                .WithMany()
                .HasForeignKey(c => c.PosteId)
                .OnDelete(DeleteBehavior.Restrict);

            entite.Ignore(c => c.DebutHorodate);
            entite.Ignore(c => c.FinHorodate);
        });

        modelBuilder.Entity<JournalAudit>(entite =>
        {
            entite.HasIndex(j => new { j.NomEntite, j.CleEntite });
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Horodater();
        var fabriquesAudit = PreparerAudit();

        var resultat = await base.SaveChangesAsync(cancellationToken);

        if (fabriquesAudit.Count > 0)
        {
            JournauxAudit.AddRange(fabriquesAudit.Select(f => f()));
            resultat += await base.SaveChangesAsync(cancellationToken);
        }

        return resultat;
    }

    public override int SaveChanges()
    {
        Horodater();
        var fabriquesAudit = PreparerAudit();

        var resultat = base.SaveChanges();

        if (fabriquesAudit.Count > 0)
        {
            JournauxAudit.AddRange(fabriquesAudit.Select(f => f()));
            resultat += base.SaveChanges();
        }

        return resultat;
    }

    private void Horodater()
    {
        var maintenant = DateTime.UtcNow;

        foreach (var entree in ChangeTracker.Entries<EntiteBase>())
        {
            switch (entree.State)
            {
                case EntityState.Added:
                    entree.Entity.DateCreation = maintenant;
                    break;
                case EntityState.Modified:
                    entree.Entity.DateModification = maintenant;
                    break;
            }
        }
    }

    /// <summary>
    /// Capture les données d'audit nécessaires (avant/après) avant l'appel à <c>base.SaveChanges</c>,
    /// et retourne des fabriques différées permettant de récupérer la clé primaire définitive
    /// (générée par la base pour les entités nouvellement créées) une fois la sauvegarde effectuée.
    /// </summary>
    private List<Func<JournalAudit>> PreparerAudit()
    {
        var maintenant = DateTime.UtcNow;
        var fabriques = new List<Func<JournalAudit>>();

        foreach (var entree in ChangeTracker.Entries())
        {
            if (entree.Entity is JournalAudit || entree.State is EntityState.Detached or EntityState.Unchanged)
            {
                continue;
            }

            var action = entree.State switch
            {
                EntityState.Added => TypeActionAudit.Creation,
                EntityState.Modified => TypeActionAudit.Modification,
                EntityState.Deleted => TypeActionAudit.Suppression,
                _ => (TypeActionAudit?)null
            };

            if (action is null)
            {
                continue;
            }

            string? valeursAvant = action != TypeActionAudit.Creation
                ? SerialiserProprietes(entree.Properties, utiliserValeurOriginale: true)
                : null;

            string? valeursApres = action != TypeActionAudit.Suppression
                ? SerialiserProprietes(entree.Properties, utiliserValeurOriginale: false)
                : null;

            var nomEntite = entree.Entity.GetType().Name;

            // Pour une suppression, l'entrée devient Detached après la sauvegarde : la clé doit
            // être capturée immédiatement. Pour une création, elle n'est générée par la base
            // qu'après la sauvegarde : la lecture doit donc être différée.
            if (action == TypeActionAudit.Suppression)
            {
                var cleCapturee = entree.Properties
                    .Where(p => p.Metadata.IsPrimaryKey())
                    .Select(p => p.CurrentValue?.ToString() ?? string.Empty)
                    .FirstOrDefault() ?? string.Empty;

                fabriques.Add(() => new JournalAudit
                {
                    DateAction = maintenant,
                    UtilisateurId = UtilisateurCourantId,
                    NomEntite = nomEntite,
                    CleEntite = cleCapturee,
                    Action = action.Value,
                    ValeursAvant = valeursAvant,
                    ValeursApres = valeursApres
                });
            }
            else
            {
                fabriques.Add(() =>
                {
                    var cleEntite = entree.Properties
                        .Where(p => p.Metadata.IsPrimaryKey())
                        .Select(p => p.CurrentValue?.ToString() ?? string.Empty)
                        .FirstOrDefault() ?? string.Empty;

                    return new JournalAudit
                    {
                        DateAction = maintenant,
                        UtilisateurId = UtilisateurCourantId,
                        NomEntite = nomEntite,
                        CleEntite = cleEntite,
                        Action = action.Value,
                        ValeursAvant = valeursAvant,
                        ValeursApres = valeursApres
                    };
                });
            }
        }

        return fabriques;
    }

    private static readonly JsonSerializerOptions OptionsJsonAudit = new()
    {
        Encoder = JavaScriptEncoder.Create(
            System.Text.Unicode.UnicodeRanges.BasicLatin,
            System.Text.Unicode.UnicodeRanges.Latin1Supplement,
            System.Text.Unicode.UnicodeRanges.LatinExtendedA)
    };

    private static string SerialiserProprietes(
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry> proprietes,
        bool utiliserValeurOriginale)
    {
        var valeurs = proprietes.ToDictionary(
            p => p.Metadata.Name,
            p => utiliserValeurOriginale ? p.OriginalValue : p.CurrentValue);
        return JsonSerializer.Serialize(valeurs, OptionsJsonAudit);
    }
}
