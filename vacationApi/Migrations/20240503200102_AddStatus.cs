using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vacationApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "VacationEntry");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "VacationEntry",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Timestamp",
                table: "Employees",
                type: "TEXT",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "VacationEntry");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "VacationEntry",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Employees",
                type: "BLOB",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldRowVersion: true,
                oldNullable: true);
        }
    }
}
