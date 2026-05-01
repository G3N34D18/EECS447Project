using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArenaSync.Web.Migrations
{
    /// <inheritdoc />
    public partial class Add_TeamEventRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamEventRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SourceEventId = table.Column<int>(type: "int", nullable: true),
                    TargetEventId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamEventRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamEventRequests_Events_SourceEventId",
                        column: x => x.SourceEventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamEventRequests_Events_TargetEventId",
                        column: x => x.TargetEventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamEventRequests_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamEventRequests_SourceEventId",
                table: "TeamEventRequests",
                column: "SourceEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamEventRequests_TargetEventId",
                table: "TeamEventRequests",
                column: "TargetEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamEventRequests_TeamId",
                table: "TeamEventRequests",
                column: "TeamId");

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[TeamReassignmentRequests]') IS NOT NULL
                BEGIN
                    INSERT INTO [TeamEventRequests]
                        ([TeamId], [Type], [SourceEventId], [TargetEventId], [Reason], [Status], [SubmittedAt], [ResolvedAt], [ResolutionNote])
                    SELECT
                        [r].[TeamId],
                        0,
                        [source].[EventId],
                        [r].[RequestedEventId],
                        [r].[Reason],
                        [r].[Status],
                        [r].[SubmittedAt],
                        NULL,
                        N''
                    FROM [TeamReassignmentRequests] AS [r]
                    OUTER APPLY (
                        SELECT CASE WHEN COUNT(*) = 1 THEN MAX([p].[EventId]) ELSE NULL END AS [EventId]
                        FROM [ParticipatesIn] AS [p]
                        WHERE [p].[TeamId] = [r].[TeamId]
                            AND [p].[EventId] <> [r].[RequestedEventId]
                    ) AS [source]
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM [TeamEventRequests] AS [existing]
                        WHERE [existing].[TeamId] = [r].[TeamId]
                            AND [existing].[Type] = 0
                            AND (
                                ([existing].[SourceEventId] IS NULL AND [source].[EventId] IS NULL)
                                OR [existing].[SourceEventId] = [source].[EventId]
                            )
                            AND [existing].[TargetEventId] = [r].[RequestedEventId]
                            AND [existing].[Status] = [r].[Status]
                    );
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamEventRequests");
        }
    }
}
