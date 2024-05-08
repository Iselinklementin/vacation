using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vacationApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestampColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Employees",
                type: "BLOB",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Employees");
        }
    }
}
