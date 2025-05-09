using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPost_Bot.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Group_Id", x => x.GroupId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_GroupID",
                table: "Posts",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupID",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Posts_GroupID",
                table: "Posts");
        }
    }
}
