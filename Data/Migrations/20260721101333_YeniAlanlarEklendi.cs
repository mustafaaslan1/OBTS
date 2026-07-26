using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBTS_Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class YeniAlanlarEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BildirenKisi",
                table: "Arizalar",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CozumTarihi",
                table: "Arizalar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirmaGonderimTarihi",
                table: "Arizalar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MudahaleBaslangicTarihi",
                table: "Arizalar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OdaNo",
                table: "Arizalar",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BildirenKisi",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "CozumTarihi",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "FirmaGonderimTarihi",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "MudahaleBaslangicTarihi",
                table: "Arizalar");

            migrationBuilder.DropColumn(
                name: "OdaNo",
                table: "Arizalar");
        }
    }
}
