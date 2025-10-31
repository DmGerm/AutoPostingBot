using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPost_Bot.Migrations
{
    /// <inheritdoc />
    public partial class StructureChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "BotID",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "GroupID",
                table: "Posts",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_GroupID",
                table: "Posts",
                newName: "IX_Posts_GroupId");

            migrationBuilder.AlterColumn<long>(
                name: "GroupId",
                table: "Posts",
                type: "TEXT",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BotToken",
                table: "Posts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Bot_token", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "BotGroups",
                columns: table => new
                {
                    BotsToken = table.Column<string>(type: "TEXT", nullable: false),
                    GroupsGroupId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotGroups", x => new { x.BotsToken, x.GroupsGroupId });
                    table.ForeignKey(
                        name: "FK_BotGroups_Bots_BotsToken",
                        column: x => x.BotsToken,
                        principalTable: "Bots",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotGroups_Groups_GroupsGroupId",
                        column: x => x.GroupsGroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BotToken",
                table: "Posts",
                column: "BotToken");

            migrationBuilder.CreateIndex(
                name: "IX_BotGroups_GroupsGroupId",
                table: "BotGroups",
                column: "GroupsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Bots_Token",
                table: "Bots",
                column: "Token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Bots_BotToken",
                table: "Posts",
                column: "BotToken",
                principalTable: "Bots",
                principalColumn: "Token",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupId",
                table: "Posts",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Bots_BotToken",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "BotGroups");

            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropIndex(
                name: "IX_Posts_BotToken",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "BotToken",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Posts",
                newName: "GroupID");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_GroupId",
                table: "Posts",
                newName: "IX_Posts_GroupID");

            migrationBuilder.AlterColumn<long>(
                name: "GroupID",
                table: "Posts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "BotID",
                table: "Posts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
