using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nba_app.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArenaId",
                table: "Game",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AwayTeamId",
                table: "Game",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "HomeTeamId",
                table: "Game",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "GameReferee",
                columns: table => new
                {
                    GamesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefereesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameReferee", x => new { x.GamesId, x.RefereesId });
                    table.ForeignKey(
                        name: "FK_GameReferee_Game_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameReferee_Referee_RefereesId",
                        column: x => x.RefereesId,
                        principalTable: "Referee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_ArenaId",
                table: "Game",
                column: "ArenaId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_AwayTeamId",
                table: "Game",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_HomeTeamId",
                table: "Game",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GameReferee_RefereesId",
                table: "GameReferee",
                column: "RefereesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Arena_ArenaId",
                table: "Game",
                column: "ArenaId",
                principalTable: "Arena",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Team_AwayTeamId",
                table: "Game",
                column: "AwayTeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Team_HomeTeamId",
                table: "Game",
                column: "HomeTeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Arena_ArenaId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Team_AwayTeamId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Team_HomeTeamId",
                table: "Game");

            migrationBuilder.DropTable(
                name: "GameReferee");

            migrationBuilder.DropIndex(
                name: "IX_Game_ArenaId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Game_AwayTeamId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Game_HomeTeamId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "ArenaId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "AwayTeamId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "HomeTeamId",
                table: "Game");
        }
    }
}
