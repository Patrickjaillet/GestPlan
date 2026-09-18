using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestPlan.Data.Export;

/// <inheritdoc cref="IServiceExportRapportConformite"/>
public class ServiceExportRapportConformite : IServiceExportRapportConformite
{
    public byte[] GenererPdf(string nomSite, DateOnly debutPeriode, DateOnly finPeriode, IReadOnlyList<AnomalieConformiteAffichable> anomalies)
    {
        var document = Document.Create(conteneur =>
        {
            conteneur.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontSize(11));

                page.Header().Column(colonne =>
                {
                    colonne.Item().Text("GestPlan — Rapport de conformité légale").FontSize(18).Bold();
                    colonne.Item().PaddingTop(4).Text($"{nomSite} — du {debutPeriode:dd/MM/yyyy} au {finPeriode:dd/MM/yyyy}");
                });

                page.Content().PaddingTop(20).Column(colonne =>
                {
                    colonne.Item().Text($"{anomalies.Count} anomalie(s) détectée(s)").Bold().FontSize(14);

                    if (anomalies.Count == 0)
                    {
                        colonne.Item().PaddingTop(8).Text("Aucune anomalie sur la période : le planning est conforme.");
                    }
                    else
                    {
                        colonne.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(colonnes =>
                            {
                                colonnes.RelativeColumn(2);
                                colonnes.RelativeColumn(1);
                                colonnes.RelativeColumn(3);
                            });

                            table.Header(entete =>
                            {
                                entete.Cell().Text("Employé").Bold();
                                entete.Cell().Text("Date").Bold();
                                entete.Cell().Text("Anomalie").Bold();
                            });

                            foreach (var anomalie in anomalies.OrderBy(a => a.NomEmploye).ThenBy(a => a.Date))
                            {
                                table.Cell().Text(anomalie.NomEmploye);
                                table.Cell().Text(anomalie.Date.ToString("dd/MM/yyyy"));
                                table.Cell().Text(anomalie.Libelle);
                            }
                        });
                    }
                });

                page.Footer().AlignCenter().Text(texte =>
                {
                    texte.Span("Généré le ");
                    texte.Span(DateTime.Now.ToString("dd/MM/yyyy à HH:mm"));
                });
            });
        });

        return document.GeneratePdf();
    }
}
