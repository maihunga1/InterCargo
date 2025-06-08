using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterCargo.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestIdToQuotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "Quotations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Quotations");
        }
    }
}
