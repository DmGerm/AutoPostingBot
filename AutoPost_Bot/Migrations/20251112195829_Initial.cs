using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPost_Bot.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    BotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Bot_Id", x => x.BotId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Group_Id", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "BotGroups",
                columns: table => new
                {
                    BotsBotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GroupsGroupId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotGroups", x => new { x.BotsBotId, x.GroupsGroupId });
                    table.ForeignKey(
                        name: "FK_BotGroups_Bots_BotsBotId",
                        column: x => x.BotsBotId,
                        principalTable: "Bots",
                        principalColumn: "BotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotGroups_Groups_GroupsGroupId",
                        column: x => x.GroupsGroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PostText = table.Column<string>(type: "TEXT", nullable: true),
                    PostDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Days = table.Column<int>(type: "INTEGER", nullable: false),
                    RepeatDays = table.Column<int>(type: "INTEGER", nullable: false),
                    RepeatHours = table.Column<int>(type: "INTEGER", nullable: false),
                    RepeatMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    BotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GroupId = table.Column<long>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Post_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Bots_BotId",
                        column: x => x.BotId,
                        principalTable: "Bots",
                        principalColumn: "BotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotGroups_GroupsGroupId",
                table: "BotGroups",
                column: "GroupsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Bots_Token",
                table: "Bots",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BotId",
                table: "Posts",
                column: "BotId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_GroupId",
                table: "Posts",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Id",
                table: "Posts",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotGroups");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
