using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBTS_Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class DetayliAlanlarEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DahiliNo",
                table: "Arizalar",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kategori",
                table: "Arizalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OncelikDerecesi",
                table: "Arizalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "UcusOperasyonunuEtkiliyorMu",
                table: "Arizalar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UrunMarkaModel",
                table: "Arizalar",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DahiliNo",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "Kategori",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "OncelikDerecesi",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "UcusOperasyonunuEtkiliyorMu",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "UrunMarkaModel",
                table: "Arizalar");
        }
    }
}
