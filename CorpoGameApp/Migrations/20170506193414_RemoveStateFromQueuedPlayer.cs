using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorpoGameApp.Migrations
{
    public partial class RemoveStateFromQueuedPlayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerQueueItems_QueueItemState_StateId",
                table: "PlayerQueueItems");

            migrationBuilder.DropTable(
                name: "QueueItemState");

            migrationBuilder.DropIndex(
                name: "IX_PlayerQueueItems_StateId",
                table: "PlayerQueueItems");

            migrationBuilder.DropColumn(
                name: "Played",
                table: "PlayerQueueItems");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "PlayerQueueItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedTime",
                table: "PlayerQueueItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinedTime",
                table: "PlayerQueueItems");

            migrationBuilder.AddColumn<bool>(
                name: "Played",
                table: "PlayerQueueItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "PlayerQueueItems",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQueueItems_StateId",
                table: "PlayerQueueItems",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerQueueItems_QueueItemState_StateId",
                table: "PlayerQueueItems",
                column: "StateId",
                principalTable: "QueueItemState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
