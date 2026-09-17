using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestPlan.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModeleMultiSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Sites",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodePostal",
                table: "Sites",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuseauHoraire",
                table: "Sites",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HeureFermeture",
                table: "Sites",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HeureOuverture",
                table: "Sites",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ville",
                table: "Sites",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployesSites",
                columns: table => new
                {
                    EmployeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployesSites", x => new { x.EmployeId, x.SiteId });
                    table.ForeignKey(
                        name: "FK_EmployesSites_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployesSites_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReglesConformite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReposQuotidienMinimum = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    ReposHebdomadaireMinimum = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    DureeMaximaleQuotidienne = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    DureeMaximaleHebdomadaire = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    DureeTravailContinuAvantPause = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    DureePauseObligatoire = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    JoursConsecutifsMaximum = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglesConformite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReglesConformite_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployesSites_SiteId",
                table: "EmployesSites",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ReglesConformite_SiteId",
                table: "ReglesConformite",
                column: "SiteId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployesSites");

            migrationBuilder.DropTable(
                name: "ReglesConformite");

            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "CodePostal",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "FuseauHoraire",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "HeureFermeture",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "HeureOuverture",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "Ville",
                table: "Sites");
        }
    }
}
