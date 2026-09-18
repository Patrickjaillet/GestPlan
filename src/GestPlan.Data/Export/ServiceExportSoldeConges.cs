using GestPlan.Core.Conges;
using GestPlan.Core.Entites;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestPlan.Data.Export;

/// <inheritdoc cref="IServiceExportSoldeConges"/>
public class ServiceExportSoldeConges : IServiceExportSoldeConges
{
    public byte[] GenererPdf(Employe employe, int anneeReference, SoldeConges solde, IReadOnlyList<Absence> absencesDeLaPeriode)
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
                    colonne.Item().Text("GestPlan — Solde de congés").FontSize(18).Bold();
                    colonne.Item().PaddingTop(4).Text($"{employe.Prenom} {employe.Nom} — Année {anneeReference}");
                });

                page.Content().PaddingTop(20).Column(colonne =>
                {
                    colonne.Item().Text("Résumé").Bold().FontSize(14);
                    colonne.Item().PaddingTop(8).Table(table =>
                    {
                        table.ColumnsDefinition(colonnes =>
                        {
                            colonnes.RelativeColumn();
                            colonnes.RelativeColumn();
                        });

                        table.Cell().Text("Jours acquis");
                        table.Cell().Text($"{solde.JoursAcquis:0.##} j");
                        table.Cell().Text("Jours pris");
                        table.Cell().Text($"{solde.JoursPris:0.##} j");
                        table.Cell().Text("Solde restant").Bold();
                        table.Cell().Text($"{solde.JoursRestants:0.##} j").Bold();
                    });

                    colonne.Item().PaddingTop(20).Text("Détail des absences").Bold().FontSize(14);

                    if (absencesDeLaPeriode.Count == 0)
                    {
                        colonne.Item().PaddingTop(8).Text("Aucune absence sur la période.");
                    }
                    else
                    {
                        colonne.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(colonnes =>
                            {
                                colonnes.RelativeColumn(2);
                                colonnes.RelativeColumn(2);
                                colonnes.RelativeColumn(2);
                                colonnes.RelativeColumn(1);
                                colonnes.RelativeColumn(2);
                            });

                            table.Header(entete =>
                            {
                                entete.Cell().Text("Type").Bold();
                                entete.Cell().Text("Début").Bold();
                                entete.Cell().Text("Fin").Bold();
                                entete.Cell().Text("Jours").Bold();
                                entete.Cell().Text("Statut").Bold();
                            });

                            foreach (var absence in absencesDeLaPeriode.OrderBy(a => a.DateDebut))
                            {
                                table.Cell().Text(absence.TypeAbsence.Nom);
                                table.Cell().Text(absence.DateDebut.ToString("dd/MM/yyyy"));
                                table.Cell().Text(absence.DateFin.ToString("dd/MM/yyyy"));
                                table.Cell().Text(absence.NombreJours.ToString());
                                table.Cell().Text(absence.Statut.ToString());
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
