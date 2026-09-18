using GestPlan.Core.Conges;
using GestPlan.Core.Entites;

namespace GestPlan.Data.Export;

/// <summary>
/// Génère l'export PDF du solde de congés d'un employé pour une année de référence.
/// </summary>
public interface IServiceExportSoldeConges
{
    byte[] GenererPdf(Employe employe, int anneeReference, SoldeConges solde, IReadOnlyList<Absence> absencesDeLaPeriode);
}
