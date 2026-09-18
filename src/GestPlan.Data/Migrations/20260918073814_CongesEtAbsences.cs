using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestPlan.Data.Migrations
{
    /// <inheritdoc />
    public partial class CongesEtAbsences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReglesAcquisitionConges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    JoursAcquisParMois = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    MoisDebutPeriodeReference = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglesAcquisitionConges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReglesAcquisitionConges_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TypesAbsence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    DecompteDuSolde = table.Column<bool>(type: "INTEGER", nullable: false),
                    CouleurAffichage = table.Column<string>(type: "TEXT", nullable: true),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesAbsence", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Absences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeAbsenceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDebut = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    Motif = table.Column<string>(type: "TEXT", nullable: true),
                    ValideParUtilisateurId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateValidation = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Absences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Absences_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Absences_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Absences_TypesAbsence_TypeAbsenceId",
                        column: x => x.TypeAbsenceId,
                        principalTable: "TypesAbsence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Absences_Utilisateurs_ValideParUtilisateurId",
                        column: x => x.ValideParUtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Absences_EmployeId_DateDebut_DateFin",
                table: "Absences",
                columns: new[] { "EmployeId", "DateDebut", "DateFin" });

            migrationBuilder.CreateIndex(
                name: "IX_Absences_SiteId",
                table: "Absences",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_TypeAbsenceId",
                table: "Absences",
                column: "TypeAbsenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_ValideParUtilisateurId",
                table: "Absences",
                column: "ValideParUtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_ReglesAcquisitionConges_SiteId",
                table: "ReglesAcquisitionConges",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesAbsence_Nom",
                table: "TypesAbsence",
                column: "Nom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Absences");

            migrationBuilder.DropTable(
                name: "ReglesAcquisitionConges");

            migrationBuilder.DropTable(
                name: "TypesAbsence");
        }
    }
}
