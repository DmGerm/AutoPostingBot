using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPost_Bot.Migrations
{
    /// <inheritdoc />
    public partial class _4Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
