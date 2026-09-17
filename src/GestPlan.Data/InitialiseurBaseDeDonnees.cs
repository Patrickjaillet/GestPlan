using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GestPlan.Data;

/// <summary>
/// Applique les migrations EF Core en attente au démarrage de l'application,
/// après avoir sauvegardé une copie horodatée de la base existante.
/// </summary>
public class InitialiseurBaseDeDonnees(GestPlanDbContext contexte, ILogger<InitialiseurBaseDeDonnees> logger)
{
    public async Task InitialiserAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(CheminsBaseDeDonnees.DossierDonnees);

        var migrationsEnAttente = (await contexte.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        if (migrationsEnAttente.Count == 0)
        {
            logger.LogInformation("Base de données à jour, aucune migration à appliquer.");
            return;
        }

        SauvegarderBaseExistante();

        logger.LogInformation(
            "Application de {Nombre} migration(s) en attente : {Migrations}",
            migrationsEnAttente.Count,
            string.Join(", ", migrationsEnAttente));

        await contexte.Database.MigrateAsync(cancellationToken);
    }

    private void SauvegarderBaseExistante()
    {
        if (!File.Exists(CheminsBaseDeDonnees.CheminBase))
        {
            return;
        }

        Directory.CreateDirectory(CheminsBaseDeDonnees.DossierSauvegardes);

        var horodatage = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var cheminSauvegarde = Path.Combine(CheminsBaseDeDonnees.DossierSauvegardes, $"gestplan-{horodatage}.db.bak");

        File.Copy(CheminsBaseDeDonnees.CheminBase, cheminSauvegarde, overwrite: false);

        logger.LogInformation("Sauvegarde de la base créée avant migration : {Chemin}", cheminSauvegarde);
    }
}
