using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestPlan.Data.Migrations
{
    /// <inheritdoc />
    public partial class GestionEmployesEtContrats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Employes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheminPhoto",
                table: "Employes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodePostal",
                table: "Employes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateNaissance",
                table: "Employes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ville",
                table: "Employes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContratPrecedentId",
                table: "Contrats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployesPostes",
                columns: table => new
                {
                    EmployeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PosteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployesPostes", x => new { x.EmployeId, x.PosteId });
                    table.ForeignKey(
                        name: "FK_EmployesPostes_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployesPostes_Postes_PosteId",
                        column: x => x.PosteId,
                        principalTable: "Postes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Indisponibilites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeId = table.Column<int>(type: "INTEGER", nullable: false),
                    JourSemaine = table.Column<int>(type: "INTEGER", nullable: false),
                    HeureDebut = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    HeureFin = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Motif = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indisponibilites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indisponibilites_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contrats_ContratPrecedentId",
                table: "Contrats",
                column: "ContratPrecedentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployesPostes_PosteId",
                table: "EmployesPostes",
                column: "PosteId");

            migrationBuilder.CreateIndex(
                name: "IX_Indisponibilites_EmployeId",
                table: "Indisponibilites",
                column: "EmployeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contrats_Contrats_ContratPrecedentId",
                table: "Contrats",
                column: "ContratPrecedentId",
                principalTable: "Contrats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contrats_Contrats_ContratPrecedentId",
                table: "Contrats");

            migrationBuilder.DropTable(
                name: "EmployesPostes");

            migrationBuilder.DropTable(
                name: "Indisponibilites");

            migrationBuilder.DropIndex(
                name: "IX_Contrats_ContratPrecedentId",
                table: "Contrats");

            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "CheminPhoto",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "CodePostal",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "DateNaissance",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "Ville",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "ContratPrecedentId",
                table: "Contrats");
        }
    }
}
