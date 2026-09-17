using GestPlan.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestPlan.Data;

/// <summary>
/// Enregistre le contexte EF Core, l'Unit of Work et l'initialiseur de base de données
/// dans le conteneur d'injection de dépendances.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AjouterGestPlanData(this IServiceCollection services)
    {
        services.AddDbContext<GestPlanDbContext>(options =>
            options.UseSqlite(CheminsBaseDeDonnees.ChaineConnexion));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<InitialiseurBaseDeDonnees>();

        return services;
    }
}
