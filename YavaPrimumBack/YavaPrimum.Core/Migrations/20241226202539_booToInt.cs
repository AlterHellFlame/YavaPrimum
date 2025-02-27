﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YavaPrimum.Core.Migrations
{
    /// <inheritdoc />
    public partial class booToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterviewStatus",
                table: "Candidate",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewStatus",
                table: "Candidate");
        }
    }
}
