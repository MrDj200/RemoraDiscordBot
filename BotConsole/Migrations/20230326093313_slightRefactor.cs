using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotConsole.Migrations
{
    /// <inheritdoc />
    public partial class slightRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerID",
                table: "SavedMessages",
                newName: "InvokerID");

            migrationBuilder.AddColumn<string>(
                name: "AuthorID",
                table: "SavedMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorID",
                table: "SavedMessages");

            migrationBuilder.RenameColumn(
                name: "InvokerID",
                table: "SavedMessages",
                newName: "OwnerID");
        }
    }
}
