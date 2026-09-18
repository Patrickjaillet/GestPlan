using GestPlan.Core.Conformite;

namespace GestPlan.Data.Export;

/// <summary>
/// Génère l'export PDF du rapport de conformité légale (liste des anomalies détectées
/// sur une période, pour un site donné).
/// </summary>
public interface IServiceExportRapportConformite
{
    byte[] GenererPdf(string nomSite, DateOnly debutPeriode, DateOnly finPeriode, IReadOnlyList<AnomalieConformiteAffichable> anomalies);
}

/// <summary>
/// Anomalie de conformité enrichie du nom de l'employé et d'un libellé prêt à afficher,
/// pour l'écran de rapport et son export PDF.
/// </summary>
public record AnomalieConformiteAffichable(TypeAnomalieConformite Type, string NomEmploye, DateOnly Date, string Libelle);
