using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application_Educative.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTrancheEtudiant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lu",
                table: "Notifications",
                newName: "Lue");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Notifications",
                newName: "DateCreation");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Etudiants",
                newName: "Tranche");

            migrationBuilder.RenameColumn(
                name: "EnrollmentDate",
                table: "Etudiants",
                newName: "DateCreation");

            migrationBuilder.RenameColumn(
                name: "Course",
                table: "Etudiants",
                newName: "Specialite");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Etudiants",
                newName: "EtudiantId");

            migrationBuilder.RenameColumn(
                name: "EstPresent",
                table: "Absences",
                newName: "Justifiee");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Absences",
                newName: "DateSaisie");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Professeurs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinataireId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinataireRole",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lien",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Matieres",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModification",
                table: "Etudiants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Etat",
                table: "Etudiants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Etudiants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotDePasse",
                table: "Etudiants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "Etudiants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "Etudiants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "Etudiants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Commentaire",
                table: "Absences",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAbsence",
                table: "Absences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "EtudiantId",
                table: "Absences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatiereId",
                table: "Absences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfesseurId",
                table: "Absences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Absences",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Absences_EtudiantId",
                table: "Absences",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_MatiereId",
                table: "Absences",
                column: "MatiereId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_ProfesseurId",
                table: "Absences",
                column: "ProfesseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_SectionId",
                table: "Absences",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_Etudiants_EtudiantId",
                table: "Absences",
                column: "EtudiantId",
                principalTable: "Etudiants",
                principalColumn: "EtudiantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_Matieres_MatiereId",
                table: "Absences",
                column: "MatiereId",
                principalTable: "Matieres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_Professeurs_ProfesseurId",
                table: "Absences",
                column: "ProfesseurId",
                principalTable: "Professeurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_Sections_SectionId",
                table: "Absences",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Absences_Etudiants_EtudiantId",
                table: "Absences");

            migrationBuilder.DropForeignKey(
                name: "FK_Absences_Matieres_MatiereId",
                table: "Absences");

            migrationBuilder.DropForeignKey(
                name: "FK_Absences_Professeurs_ProfesseurId",
                table: "Absences");

            migrationBuilder.DropForeignKey(
                name: "FK_Absences_Sections_SectionId",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_EtudiantId",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_MatiereId",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_ProfesseurId",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_SectionId",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Professeurs");

            migrationBuilder.DropColumn(
                name: "DestinataireId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DestinataireRole",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Lien",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DateModification",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Etat",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "MotDePasse",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Commentaire",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "DateAbsence",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "EtudiantId",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "MatiereId",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "ProfesseurId",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Absences");

            migrationBuilder.RenameColumn(
                name: "Lue",
                table: "Notifications",
                newName: "Lu");

            migrationBuilder.RenameColumn(
                name: "DateCreation",
                table: "Notifications",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "Tranche",
                table: "Etudiants",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Specialite",
                table: "Etudiants",
                newName: "Course");

            migrationBuilder.RenameColumn(
                name: "DateCreation",
                table: "Etudiants",
                newName: "EnrollmentDate");

            migrationBuilder.RenameColumn(
                name: "EtudiantId",
                table: "Etudiants",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Justifiee",
                table: "Absences",
                newName: "EstPresent");

            migrationBuilder.RenameColumn(
                name: "DateSaisie",
                table: "Absences",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Matieres",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
