using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptographyTest.Migrations
{
    /// <inheritdoc />
    public partial class CaseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Users_DetectiveId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Users_SupervisorId",
                table: "Cases");

            migrationBuilder.AlterColumn<Guid>(
                name: "SupervisorId",
                table: "Cases",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "DetectiveId",
                table: "Cases",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Cases",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Users_DetectiveId",
                table: "Cases",
                column: "DetectiveId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Users_SupervisorId",
                table: "Cases",
                column: "SupervisorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Users_DetectiveId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Users_SupervisorId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cases");

            migrationBuilder.AlterColumn<Guid>(
                name: "SupervisorId",
                table: "Cases",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DetectiveId",
                table: "Cases",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Users_DetectiveId",
                table: "Cases",
                column: "DetectiveId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Users_SupervisorId",
                table: "Cases",
                column: "SupervisorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
