using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotGame.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReelStrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    ColumnIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReelStrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReelStrips_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    BetAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalWin = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    FinalMatrixJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spins_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReelSymbols",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReelStripId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReelSymbols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReelSymbols_ReelStrips_ReelStripId",
                        column: x => x.ReelStripId,
                        principalTable: "ReelStrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReelStrips_GameId_ColumnIndex",
                table: "ReelStrips",
                columns: new[] { "GameId", "ColumnIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReelSymbols_ReelStripId_Position",
                table: "ReelSymbols",
                columns: new[] { "ReelStripId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spins_GameId",
                table: "Spins",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReelSymbols");

            migrationBuilder.DropTable(
                name: "Spins");

            migrationBuilder.DropTable(
                name: "ReelStrips");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
