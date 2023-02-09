using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManager.Migrations
{
    /// <inheritdoc />
    public partial class _05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DrawingTitle",
                table: "DrawingIssueEntry",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "DrawingNumber",
                table: "DrawingIssueEntry",
                newName: "Path");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "DrawingIssues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DrawingIssues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "DrawingIssueEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "DrawingIssues");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DrawingIssues");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "DrawingIssueEntry");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "DrawingIssueEntry",
                newName: "DrawingTitle");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "DrawingIssueEntry",
                newName: "DrawingNumber");
        }
    }
}
