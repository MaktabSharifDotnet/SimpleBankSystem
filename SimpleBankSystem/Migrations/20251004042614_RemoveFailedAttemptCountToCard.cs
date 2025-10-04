using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleBankSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFailedAttemptCountToCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedAttemptCount",
                table: "Cards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedAttemptCount",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
