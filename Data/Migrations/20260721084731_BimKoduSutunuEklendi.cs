using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBTS_Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class BimKoduSutunuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BimKodu",
                table: "Arizalar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BimKodu",
                table: "Arizalar");
        }
    }
}
