namespace GestPlan.Data.Repositories;

/// <summary>
/// Crée des instances de <see cref="IUnitOfWork"/> à la demande, chacune adossée à son propre
/// périmètre de dépendances (<c>IServiceScope</c>) et donc à son propre <see cref="GestPlanDbContext"/>.
/// À utiliser par les consommateurs qui ne vivent pas eux-mêmes dans un scope applicatif
/// (par exemple les view-models WPF, résolus depuis le conteneur racine).
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Crée un nouveau <see cref="IUnitOfWork"/>. L'appelant est responsable de disposer
    /// l'instance retournée (elle implémente <see cref="IDisposable"/> via son scope sous-jacent).
    /// </summary>
    IUnitOfWork Creer();
}
