using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArenaSync.Web.Migrations
{
    /// <inheritdoc />
    public partial class Add_TeamReassignmentRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamReassignmentRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    RequestedEventId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamReassignmentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamReassignmentRequests_Events_RequestedEventId",
                        column: x => x.RequestedEventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamReassignmentRequests_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamReassignmentRequests_RequestedEventId",
                table: "TeamReassignmentRequests",
                column: "RequestedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamReassignmentRequests_TeamId",
                table: "TeamReassignmentRequests",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamReassignmentRequests");
        }
    }
}
