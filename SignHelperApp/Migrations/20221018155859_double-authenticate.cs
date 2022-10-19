using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignHelperApp.Migrations
{
    public partial class doubleauthenticate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "SignRequests",
                newName: "SignerPhoneNumber");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpireIn",
                table: "SignRequests",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ConfirmCodeExpireIn",
                table: "SignRequests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                table: "SignRequests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SignerEmail",
                table: "SignRequests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmCodeExpireIn",
                table: "SignRequests");

            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                table: "SignRequests");

            migrationBuilder.DropColumn(
                name: "SignerEmail",
                table: "SignRequests");

            migrationBuilder.RenameColumn(
                name: "SignerPhoneNumber",
                table: "SignRequests",
                newName: "Email");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpireIn",
                table: "SignRequests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
