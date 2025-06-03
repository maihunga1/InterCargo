using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterCargo.Migrations
{
    /// <inheritdoc />
    public partial class AddedListChargeItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedChargeItemsJson",
                table: "Quotations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedChargeItemsJson",
                table: "Quotations");
        }
    }
}
