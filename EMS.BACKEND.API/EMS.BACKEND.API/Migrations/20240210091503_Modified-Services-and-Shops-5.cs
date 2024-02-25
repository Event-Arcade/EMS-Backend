using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.BACKEND.API.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedServicesandShops5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_shops_ShopId",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "FK_shops_AspNetUsers_OwnerId",
                table: "shops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shops",
                table: "shops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_services",
                table: "services");

            migrationBuilder.RenameTable(
                name: "shops",
                newName: "Shops");

            migrationBuilder.RenameTable(
                name: "services",
                newName: "Services");

            migrationBuilder.RenameIndex(
                name: "IX_shops_OwnerId",
                table: "Shops",
                newName: "IX_Shops_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_services_ShopId",
                table: "Services",
                newName: "IX_Services_ShopId");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Shops",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "ShopId",
                table: "Services",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shops",
                table: "Shops",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Shops_ShopId",
                table: "Services",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_AspNetUsers_OwnerId",
                table: "Shops",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Shops_ShopId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_AspNetUsers_OwnerId",
                table: "Shops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shops",
                table: "Shops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.RenameTable(
                name: "Shops",
                newName: "shops");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "services");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_OwnerId",
                table: "shops",
                newName: "IX_shops_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ShopId",
                table: "services",
                newName: "IX_services_ShopId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "shops",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ShopId",
                table: "services",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_shops",
                table: "shops",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_services",
                table: "services",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_services_shops_ShopId",
                table: "services",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_shops_AspNetUsers_OwnerId",
                table: "shops",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
