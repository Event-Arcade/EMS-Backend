using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.BACKEND.API.Migrations
{
    /// <inheritdoc />
    public partial class updatenotificationentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Notifications",
                newName: "EventType");

            migrationBuilder.AlterColumn<int>(
                name: "EntityId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EntityType",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "Notifications",
                newName: "Type");

            migrationBuilder.AlterColumn<int>(
                name: "EntityId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
