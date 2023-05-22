using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManager.Migrations
{
    /// <inheritdoc />
    public partial class _02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CreationEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LineManagerId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TasksEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Company = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DrawingIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawingIssues_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssuerId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Issued = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Responded = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Won = table.Column<bool>(type: "boolean", nullable: false),
                    TotalFee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotes_AspNetUsers_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Variations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    DateIssued = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VariationText = table.Column<string>(type: "text", nullable: false),
                    Delay = table.Column<string>(type: "text", nullable: false),
                    Charge = table.Column<string>(type: "text", nullable: false),
                    Accepted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AcceptorId = table.Column<Guid>(type: "uuid", nullable: true),
                    RaisedById = table.Column<string>(type: "text", nullable: true),
                    OriginatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Variations_AspNetUsers_RaisedById",
                        column: x => x.RaisedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Variations_Contact_AcceptorId",
                        column: x => x.AcceptorId,
                        principalTable: "Contact",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Variations_Contact_OriginatorId",
                        column: x => x.OriginatorId,
                        principalTable: "Contact",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Variations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactDrawingIssue",
                columns: table => new
                {
                    ContactsId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssuesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactDrawingIssue", x => new { x.ContactsId, x.IssuesId });
                    table.ForeignKey(
                        name: "FK_ContactDrawingIssue_Contact_ContactsId",
                        column: x => x.ContactsId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactDrawingIssue_DrawingIssues_IssuesId",
                        column: x => x.IssuesId,
                        principalTable: "DrawingIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrawingIssueEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Revision = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    DrawingIssueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingIssueEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawingIssueEntry_DrawingIssues_DrawingIssueId",
                        column: x => x.DrawingIssueId,
                        principalTable: "DrawingIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LineManagerId",
                table: "AspNetUsers",
                column: "LineManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactDrawingIssue_IssuesId",
                table: "ContactDrawingIssue",
                column: "IssuesId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawingIssueEntry_DrawingIssueId",
                table: "DrawingIssueEntry",
                column: "DrawingIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawingIssues_ProjectId",
                table: "DrawingIssues",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_IssuerId",
                table: "Quotes",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_ProjectId",
                table: "Quotes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_AcceptorId",
                table: "Variations",
                column: "AcceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_OriginatorId",
                table: "Variations",
                column: "OriginatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_ProjectId",
                table: "Variations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_RaisedById",
                table: "Variations",
                column: "RaisedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_LineManagerId",
                table: "AspNetUsers",
                column: "LineManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_LineManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ContactDrawingIssue");

            migrationBuilder.DropTable(
                name: "DrawingIssueEntry");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Variations");

            migrationBuilder.DropTable(
                name: "DrawingIssues");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LineManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreationEnabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LineManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TasksEnabled",
                table: "AspNetUsers");
        }
    }
}
