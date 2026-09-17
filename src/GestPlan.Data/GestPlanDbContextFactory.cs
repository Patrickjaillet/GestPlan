using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GestPlan.Data;

/// <summary>
/// Fabrique utilisée par les outils EF Core (<c>dotnet ef migrations add</c>, etc.)
/// pour créer un <see cref="GestPlanDbContext"/> en dehors de l'exécution de l'application.
/// </summary>
public class GestPlanDbContextFactory : IDesignTimeDbContextFactory<GestPlanDbContext>
{
    public GestPlanDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GestPlanDbContext>();
        optionsBuilder.UseSqlite("Data Source=gestplan.design.db");

        return new GestPlanDbContext(optionsBuilder.Options);
    }
}
