using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectMvc.Migrations
{
    /// <inheritdoc />
    public partial class add1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyMenuItemStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManuItemId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMenuItemStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyMenuItemStats_ManuItems_ManuItemId",
                        column: x => x.ManuItemId,
                        principalTable: "ManuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenuItemStats_ManuItemId",
                table: "DailyMenuItemStats",
                column: "ManuItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyMenuItemStats");
        }
    }
}
