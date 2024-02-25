using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.BACKEND.API.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedServicesandShops2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_shops_ShopId",
                table: "services");

            migrationBuilder.DropIndex(
                name: "IX_services_ShopId",
                table: "services");

            migrationBuilder.AlterColumn<string>(
                name: "ShopId",
                table: "services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "services",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Logitude",
                table: "services",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ShopId1",
                table: "services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_services_ShopId1",
                table: "services",
                column: "ShopId1");

            migrationBuilder.AddForeignKey(
                name: "FK_services_shops_ShopId1",
                table: "services",
                column: "ShopId1",
                principalTable: "shops",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_shops_ShopId1",
                table: "services");

            migrationBuilder.DropIndex(
                name: "IX_services_ShopId1",
                table: "services");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "services");

            migrationBuilder.DropColumn(
                name: "Logitude",
                table: "services");

            migrationBuilder.DropColumn(
                name: "ShopId1",
                table: "services");

            migrationBuilder.AlterColumn<int>(
                name: "ShopId",
                table: "services",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_services_ShopId",
                table: "services",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_services_shops_ShopId",
                table: "services",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id");
        }
    }
}
