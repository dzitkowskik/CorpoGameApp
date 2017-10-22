using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CorpoGameApp.Migrations
{
    public partial class add_player_queue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueueItemState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueItemState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerQueueItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Played = table.Column<bool>(nullable: false),
                    PlayerId = table.Column<int>(nullable: true),
                    StateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerQueueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerQueueItems_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerQueueItems_QueueItemState_StateId",
                        column: x => x.StateId,
                        principalTable: "QueueItemState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQueueItems_PlayerId",
                table: "PlayerQueueItems",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQueueItems_StateId",
                table: "PlayerQueueItems",
                column: "StateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerQueueItems");

            migrationBuilder.DropTable(
                name: "QueueItemState");
        }
    }
}
