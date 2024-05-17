using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMS.BACKEND.API.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "AdminId", "CategoryImagePath", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Automotive" },
                    { 2, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Beauty" },
                    { 3, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Construction" },
                    { 4, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Education" },
                    { 5, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Entertainment" },
                    { 6, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Food" },
                    { 7, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Health" },
                    { 8, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "IT" },
                    { 9, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Legal" },
                    { 10, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Manufacturing" },
                    { 11, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Retail" },
                    { 12, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Services" },
                    { 13, "2908e25a-b961-4dea-85b2-9ec84c1e6226", "https://via.placeholder.com/150", null, "Transportation" }
                });

            migrationBuilder.InsertData(
                table: "Shops",
                columns: new[] { "Id", "BackgroundImagePath", "Description", "Name", "OwnerId", "Rating" },
                values: new object[] { 1, "https://via.placeholder.com/150", null, "Shop 1", "9b982dc2-f99d-4c9b-b3db-c6ed2e193c98", null });

            migrationBuilder.InsertData(
                table: "ShopServices",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Price", "Rating", "ShopId" },
                values: new object[] { 1, 1, "Service 1 Description", "Service 1", 100.0, 4.5, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ShopServices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shops",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
