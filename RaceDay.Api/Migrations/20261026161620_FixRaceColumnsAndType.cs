using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDay.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixRaceColumnsAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceEntries_Participant_ParticipantID",
                table: "RaceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceEntries_Races_RacesRaceID",
                table: "RaceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceEvents_Organiser_OrganiserID",
                table: "RaceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_Participant_ParticipantID",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_Races_RaceID",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_RaceEntries_RacesRaceID",
                table: "RaceEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Results",
                table: "Results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participant",
                table: "Participant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organiser",
                table: "Organiser");

            migrationBuilder.DropColumn(
                name: "RacesRaceID",
                table: "RaceEntries");

            migrationBuilder.RenameTable(
                name: "Results",
                newName: "RaceResults");

            migrationBuilder.RenameTable(
                name: "Participant",
                newName: "Participants");

            migrationBuilder.RenameTable(
                name: "Organiser",
                newName: "Organisers");

            migrationBuilder.RenameColumn(
                name: "location",
                table: "RaceEvents",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "RaceID",
                table: "RaceEntries",
                newName: "RaceEventID");

            migrationBuilder.RenameColumn(
                name: "EntryDate",
                table: "RaceEntries",
                newName: "RegistrationDate");

            migrationBuilder.RenameIndex(
                name: "IX_Results_RaceID",
                table: "RaceResults",
                newName: "IX_RaceResults_RaceID");

            migrationBuilder.RenameIndex(
                name: "IX_Results_ParticipantID",
                table: "RaceResults",
                newName: "IX_RaceResults_ParticipantID");

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Races",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OrganiserID",
                table: "Races",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganiserID",
                table: "RaceEvents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrganiserID1",
                table: "RaceEvents",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantID",
                table: "RaceEntries",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantID",
                table: "RaceResults",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishTime",
                table: "RaceResults",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Organisers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RaceResults",
                table: "RaceResults",
                column: "ResultID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participants",
                table: "Participants",
                column: "ParticipantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organisers",
                table: "Organisers",
                column: "OrganiserID");

            migrationBuilder.CreateIndex(
                name: "IX_Races_CategoryID",
                table: "Races",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_RaceEvents_OrganiserID1",
                table: "RaceEvents",
                column: "OrganiserID1");

            migrationBuilder.CreateIndex(
                name: "IX_RaceEntries_RaceEventID",
                table: "RaceEntries",
                column: "RaceEventID");

            migrationBuilder.CreateIndex(
                name: "IX_Organisers_ApplicationUserId",
                table: "Organisers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisers_AspNetUsers_ApplicationUserId",
                table: "Organisers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceEntries_AspNetUsers_ParticipantID",
                table: "RaceEntries",
                column: "ParticipantID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceEntries_RaceEvents_RaceEventID",
                table: "RaceEntries",
                column: "RaceEventID",
                principalTable: "RaceEvents",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceEvents_AspNetUsers_OrganiserID",
                table: "RaceEvents",
                column: "OrganiserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceEvents_Organisers_OrganiserID1",
                table: "RaceEvents",
                column: "OrganiserID1",
                principalTable: "Organisers",
                principalColumn: "OrganiserID");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceResults_AspNetUsers_ParticipantID",
                table: "RaceResults",
                column: "ParticipantID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceResults_Races_RaceID",
                table: "RaceResults",
                column: "RaceID",
                principalTable: "Races",
                principalColumn: "RaceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_Categories_CategoryID",
                table: "Races",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisers_AspNetUsers_ApplicationUserId",
                table: "Organisers");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceEntries_AspNetUsers_ParticipantID",
                table: "RaceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceEntries_RaceEvents_RaceEventID",
                table: "RaceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceEvents_AspNetUsers_OrganiserID",
                table: "RaceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceEvents_Organisers_OrganiserID1",
                table: "RaceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceResults_AspNetUsers_ParticipantID",
                table: "RaceResults");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceResults_Races_RaceID",
                table: "RaceResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_Categories_CategoryID",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Races_CategoryID",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_RaceEvents_OrganiserID1",
                table: "RaceEvents");

            migrationBuilder.DropIndex(
                name: "IX_RaceEntries_RaceEventID",
                table: "RaceEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RaceResults",
                table: "RaceResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participants",
                table: "Participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organisers",
                table: "Organisers");

            migrationBuilder.DropIndex(
                name: "IX_Organisers_ApplicationUserId",
                table: "Organisers");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "OrganiserID",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "OrganiserID1",
                table: "RaceEvents");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Organisers");

            migrationBuilder.RenameTable(
                name: "RaceResults",
                newName: "Results");

            migrationBuilder.RenameTable(
                name: "Participants",
                newName: "Participant");

            migrationBuilder.RenameTable(
                name: "Organisers",
                newName: "Organiser");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "RaceEvents",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "RaceEntries",
                newName: "EntryDate");

            migrationBuilder.RenameColumn(
                name: "RaceEventID",
                table: "RaceEntries",
                newName: "RaceID");

            migrationBuilder.RenameIndex(
                name: "IX_RaceResults_RaceID",
                table: "Results",
                newName: "IX_Results_RaceID");

            migrationBuilder.RenameIndex(
                name: "IX_RaceResults_ParticipantID",
                table: "Results",
                newName: "IX_Results_ParticipantID");

            migrationBuilder.AlterColumn<int>(
                name: "OrganiserID",
                table: "RaceEvents",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "ParticipantID",
                table: "RaceEntries",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "RacesRaceID",
                table: "RaceEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParticipantID",
                table: "Results",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FinishTime",
                table: "Results",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Results",
                table: "Results",
                column: "ResultID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participant",
                table: "Participant",
                column: "ParticipantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organiser",
                table: "Organiser",
                column: "OrganiserID");

            migrationBuilder.CreateIndex(
                name: "IX_RaceEntries_RacesRaceID",
                table: "RaceEntries",
                column: "RacesRaceID");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceEntries_Participant_ParticipantID",
                table: "RaceEntries",
                column: "ParticipantID",
                principalTable: "Participant",
                principalColumn: "ParticipantID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceEntries_Races_RacesRaceID",
                table: "RaceEntries",
                column: "RacesRaceID",
                principalTable: "Races",
                principalColumn: "RaceID");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceEvents_Organiser_OrganiserID",
                table: "RaceEvents",
                column: "OrganiserID",
                principalTable: "Organiser",
                principalColumn: "OrganiserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Participant_ParticipantID",
                table: "Results",
                column: "ParticipantID",
                principalTable: "Participant",
                principalColumn: "ParticipantID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Races_RaceID",
                table: "Results",
                column: "RaceID",
                principalTable: "Races",
                principalColumn: "RaceID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
