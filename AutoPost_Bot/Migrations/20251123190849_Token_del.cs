using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPost_Bot.Migrations
{
    /// <inheritdoc />
    public partial class Token_del : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bots_Token",
                table: "Bots");

            migrationBuilder.CreateIndex(
                name: "IX_Bots_Token",
                table: "Bots",
                column: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bots_Token",
                table: "Bots");

            migrationBuilder.CreateIndex(
                name: "IX_Bots_Token",
                table: "Bots",
                column: "Token",
                unique: true);
        }
    }
}
