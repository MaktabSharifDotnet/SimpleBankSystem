using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleBankSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFailedAttemptCountToCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedAttemptCount",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedAttemptCount",
                table: "Cards");
        }
    }
}
