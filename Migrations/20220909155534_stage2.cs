using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessTals.Migrations
{
    /// <inheritdoc />
    public partial class stage2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderName",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderName",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderName",
                table: "Messages",
                newName: "Sender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sender",
                table: "Messages",
                newName: "SenderName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderName",
                table: "Messages",
                column: "SenderName");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderName",
                table: "Messages",
                column: "SenderName",
                principalTable: "Users",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
