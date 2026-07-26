using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBTS_Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciDetaylariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdSoyad",
                table: "KullaniciEkBilgileri",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DahiliHat",
                table: "KullaniciEkBilgileri",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdSoyad",
                table: "KullaniciEkBilgileri");

            migrationBuilder.DropColumn(
                name: "DahiliHat",
                table: "KullaniciEkBilgileri");
        }
    }
}
