using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterCargo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuotationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobNature",
                table: "Quotations",
                newName: "QuarantineRequirements");

            migrationBuilder.AddColumn<string>(
                name: "ImportExportType",
                table: "Quotations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PackingUnpacking",
                table: "Quotations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportExportType",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "PackingUnpacking",
                table: "Quotations");

            migrationBuilder.RenameColumn(
                name: "QuarantineRequirements",
                table: "Quotations",
                newName: "JobNature");
        }
    }
}
