using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignHelperApp.Migrations
{
    public partial class stamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignRequests_Templates_TemplateId",
                table: "SignRequests");

            migrationBuilder.RenameColumn(
                name: "SignImageName",
                table: "Templates",
                newName: "StampName");

            migrationBuilder.RenameColumn(
                name: "TemplateId",
                table: "SignRequests",
                newName: "SignTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_SignRequests_TemplateId",
                table: "SignRequests",
                newName: "IX_SignRequests_SignTemplateId");

            migrationBuilder.AddColumn<long>(
                name: "StampTemplateId",
                table: "SignRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SignRequests_Templates_SignTemplateId",
                table: "SignRequests",
                column: "SignTemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignRequests_Templates_SignTemplateId",
                table: "SignRequests");

            migrationBuilder.DropColumn(
                name: "StampTemplateId",
                table: "SignRequests");

            migrationBuilder.RenameColumn(
                name: "StampName",
                table: "Templates",
                newName: "SignImageName");

            migrationBuilder.RenameColumn(
                name: "SignTemplateId",
                table: "SignRequests",
                newName: "TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_SignRequests_SignTemplateId",
                table: "SignRequests",
                newName: "IX_SignRequests_TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_SignRequests_Templates_TemplateId",
                table: "SignRequests",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
