using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.BACKEND.API.Migrations
{
    /// <inheritdoc />
    public partial class feedbackstaticresources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedBackStaticResource_FeedBacks_FeedBackId",
                table: "FeedBackStaticResource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeedBackStaticResource",
                table: "FeedBackStaticResource");

            migrationBuilder.RenameTable(
                name: "FeedBackStaticResource",
                newName: "FeedBackStaticResources");

            migrationBuilder.RenameIndex(
                name: "IX_FeedBackStaticResource_FeedBackId",
                table: "FeedBackStaticResources",
                newName: "IX_FeedBackStaticResources_FeedBackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeedBackStaticResources",
                table: "FeedBackStaticResources",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBackStaticResources_FeedBacks_FeedBackId",
                table: "FeedBackStaticResources",
                column: "FeedBackId",
                principalTable: "FeedBacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedBackStaticResources_FeedBacks_FeedBackId",
                table: "FeedBackStaticResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeedBackStaticResources",
                table: "FeedBackStaticResources");

            migrationBuilder.RenameTable(
                name: "FeedBackStaticResources",
                newName: "FeedBackStaticResource");

            migrationBuilder.RenameIndex(
                name: "IX_FeedBackStaticResources_FeedBackId",
                table: "FeedBackStaticResource",
                newName: "IX_FeedBackStaticResource_FeedBackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeedBackStaticResource",
                table: "FeedBackStaticResource",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBackStaticResource_FeedBacks_FeedBackId",
                table: "FeedBackStaticResource",
                column: "FeedBackId",
                principalTable: "FeedBacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
