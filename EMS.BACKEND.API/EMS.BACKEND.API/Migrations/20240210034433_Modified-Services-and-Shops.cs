using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.BACKEND.API.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedServicesandShops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_platforms",
                table: "platforms");

            migrationBuilder.RenameTable(
                name: "platforms",
                newName: "services");

            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "services",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_services",
                table: "services",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shops_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_services_ShopId",
                table: "services",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_shops_OwnerId",
                table: "shops",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_services_shops_ShopId",
                table: "services",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_shops_ShopId",
                table: "services");

            migrationBuilder.DropTable(
                name: "shops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_services",
                table: "services");

            migrationBuilder.DropIndex(
                name: "IX_services_ShopId",
                table: "services");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "services");

            migrationBuilder.RenameTable(
                name: "services",
                newName: "platforms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platforms",
                table: "platforms",
                column: "Id");
        }
    }
}
