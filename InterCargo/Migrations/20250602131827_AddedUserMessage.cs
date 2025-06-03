using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterCargo.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerResponseMessage",
                table: "Quotations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerResponseStatus",
                table: "Quotations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerResponseMessage",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerResponseStatus",
                table: "Quotations");
        }
    }
}
