using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptographyTest.Migrations
{
    /// <inheritdoc />
    public partial class CaseRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tips_Cases_CaseId",
                table: "Tips");

            migrationBuilder.AlterColumn<Guid>(
                name: "CaseId",
                table: "Tips",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tips_Cases_CaseId",
                table: "Tips",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tips_Cases_CaseId",
                table: "Tips");

            migrationBuilder.AlterColumn<Guid>(
                name: "CaseId",
                table: "Tips",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Tips_Cases_CaseId",
                table: "Tips",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id");
        }
    }
}
