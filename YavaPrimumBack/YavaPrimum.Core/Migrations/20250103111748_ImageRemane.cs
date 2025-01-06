using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YavaPrimum.Core.Migrations
{
    /// <inheritdoc />
    public partial class ImageRemane : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "User",
                newName: "ImgUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgUrl",
                table: "User",
                newName: "Image");
        }
    }
}
