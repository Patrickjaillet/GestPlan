using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestPlan.Data.Migrations
{
    /// <inheritdoc />
    public partial class AuthentificationEtSecurite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateDernierChangementMotDePasse",
                table: "Utilisateurs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DerniereConnexion",
                table: "Utilisateurs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TentativesEchoueesConsecutives",
                table: "Utilisateurs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerrouilleJusqua",
                table: "Utilisateurs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDernierChangementMotDePasse",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "DerniereConnexion",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "TentativesEchoueesConsecutives",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "VerrouilleJusqua",
                table: "Utilisateurs");
        }
    }
}
