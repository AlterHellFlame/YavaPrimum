using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YavaHire.Core.Migrations
{
    /// <inheritdoc />
    public partial class MigrationBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HRs",
                columns: table => new
                {
                    HRId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRs", x => x.HRId);
                });

            migrationBuilder.CreateTable(
                name: "PersonalOfficers",
                columns: table => new
                {
                    PersonalOfficerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalOfficers", x => x.PersonalOfficerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HRs");

            migrationBuilder.DropTable(
                name: "PersonalOfficers");
        }
    }
}
