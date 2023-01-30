using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManager.Migrations
{
    /// <inheritdoc />
    public partial class _04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Variations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIssued = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VariationText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Delay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Charge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accepted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcceptorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RaisedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OriginatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Variations");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Contact");
        }
    }
}
