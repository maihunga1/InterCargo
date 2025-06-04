using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterCargo.Migrations
{
    /// <inheritdoc />
    public partial class AddedContainerType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContainerType",
                table: "Quotations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContainerType",
                table: "Quotations");
        }
    }
}
