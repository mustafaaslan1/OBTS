using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBTS_Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class IslemGecmisiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArizaIslemGecmisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArizaId = table.Column<int>(type: "int", nullable: false),
                    TeknikerAdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IslemAciklamasi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArizaIslemGecmisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArizaIslemGecmisleri_Arizalar_ArizaId",
                        column: x => x.ArizaId,
                        principalTable: "Arizalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArizaIslemGecmisleri_ArizaId",
                table: "ArizaIslemGecmisleri",
                column: "ArizaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArizaIslemGecmisleri");
        }
    }
}
