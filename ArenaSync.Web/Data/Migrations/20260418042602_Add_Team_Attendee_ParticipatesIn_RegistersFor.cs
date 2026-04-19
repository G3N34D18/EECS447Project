using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArenaSync.Web.Migrations
{
    /// <inheritdoc />
    public partial class Add_Team_Attendee_ParticipatesIn_RegistersFor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamAssignments_LockerRooms_LockerRoomId",
                table: "TeamAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeamAssignments_LockerRoomId",
                table: "TeamAssignments");

            migrationBuilder.DropColumn(
                name: "LockerRoomId",
                table: "TeamAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Teams",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Manager",
                table: "Teams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teams",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Attendees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Attendees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Attendees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAssignments_LockerId",
                table: "TeamAssignments",
                column: "LockerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAssignments_LockerRooms_LockerId",
                table: "TeamAssignments",
                column: "LockerId",
                principalTable: "LockerRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamAssignments_LockerRooms_LockerId",
                table: "TeamAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeamAssignments_LockerId",
                table: "TeamAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Manager",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<int>(
                name: "LockerRoomId",
                table: "TeamAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Attendees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Attendees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Attendees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.CreateIndex(
                name: "IX_TeamAssignments_LockerRoomId",
                table: "TeamAssignments",
                column: "LockerRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAssignments_LockerRooms_LockerRoomId",
                table: "TeamAssignments",
                column: "LockerRoomId",
                principalTable: "LockerRooms",
                principalColumn: "Id");
        }
    }
}
