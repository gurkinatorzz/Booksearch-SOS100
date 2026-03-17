using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLoanService.Migrations
{
    /// <inheritdoc />
    public partial class AddIsComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "BookLoans",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "BookLoans");
        }
    }
}
