using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPost_Bot.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PostText = table.Column<string>(type: "TEXT", nullable: true),
                    GroupID = table.Column<long>(type: "INTEGER", nullable: false),
                    PostDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RepeatDays = table.Column<int>(type: "INTEGER", nullable: false),
                    RepeatHours = table.Column<int>(type: "INTEGER", nullable: false),
                    RepeatMinutes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Post_Id", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Id",
                table: "Posts",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
