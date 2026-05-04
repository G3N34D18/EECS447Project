using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArenaSync.Web.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_LockerRooms_Venues_VenueId",
                table: "LockerRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipatesIn_Events_EventId",
                table: "ParticipatesIn");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipatesIn_Teams_TeamId",
                table: "ParticipatesIn");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistersFor_Attendees_AttendeeId",
                table: "RegistersFor");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistersFor_Events_EventId",
                table: "RegistersFor");

            migrationBuilder.DropForeignKey(
                name: "FK_SuppliesAt_Events_EventId",
                table: "SuppliesAt");

            migrationBuilder.DropForeignKey(
                name: "FK_SuppliesAt_Vendors_VendorId",
                table: "SuppliesAt");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAssignments_Events_EventId",
                table: "TeamAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAssignments_LockerRooms_LockerRoomId",
                table: "TeamAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAssignments_Teams_TeamId",
                table: "TeamAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorBooths_Venues_VenueId",
                table: "VendorBooths");

            migrationBuilder.DropIndex(
                name: "IX_TeamAssignments_LockerRoomId",
                table: "TeamAssignments");

            migrationBuilder.DropColumn(
                name: "LockerRoomId",
                table: "TeamAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Venues",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Venues",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Vendors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Vendors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Vendors",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Vendors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                name: "Name",
                table: "Events",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(1000)",
                maxLength: 1000,
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
                name: "FK_Events_Venues_VenueId",
                table: "Events",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LockerRooms_Venues_VenueId",
                table: "LockerRooms",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipatesIn_Events_EventId",
                table: "ParticipatesIn",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipatesIn_Teams_TeamId",
                table: "ParticipatesIn",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistersFor_Attendees_AttendeeId",
                table: "RegistersFor",
                column: "AttendeeId",
                principalTable: "Attendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistersFor_Events_EventId",
                table: "RegistersFor",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SuppliesAt_Events_EventId",
                table: "SuppliesAt",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SuppliesAt_Vendors_VendorId",
                table: "SuppliesAt",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAssignments_Events_EventId",
                table: "TeamAssignments",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAssignments_LockerRooms_LockerId",
                table: "TeamAssignments",
                column: "LockerId",
                principalTable: "LockerRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAssignments_Teams_TeamId",
                table: "TeamAssignments",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorBooths_Venues_VenueId",
                table: "VendorBooths",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_LockerRooms_Venues_VenueId",
                table: "LockerRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipatesIn_Events_EventId",
                table: "ParticipatesIn");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipatesIn_Teams_TeamId",
                table: "ParticipatesIn");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistersFor_Attendees_AttendeeId",
                table: "RegistersFor");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistersFor_Events_EventId",
                table: "RegistersFor");

            migrationBuilder.DropForeignKey(
                name: "FK_SuppliesAt_Events_EventId",
                table: "SuppliesAt");

            migrationBuilder.DropForeignKey(
                name: "FK_SuppliesAt_Vendors_VendorId",
                table: "SuppliesAt");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAssignments_Events_EventId",
                table: "TeamAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAssignments_LockerRooms_LockerId",
                table: "TeamAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAssignments_Teams_TeamId",
                table: "TeamAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorBooths_Venues_VenueId",
                table: "VendorBooths");

            migrationBuilder.DropIndex(
                name: "IX_TeamAssignments_LockerId",
                table: "TeamAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

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
                name: "Name",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

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
                name: "FK_Events_Venues_VenueId",
                table: "Events",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LockerRooms_Venues_VenueId",
                table: "LockerRooms",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipatesIn_Events_EventId",
                table: "ParticipatesIn",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipatesIn_Teams_TeamId",
                table: "ParticipatesIn",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistersFor_Attendees_AttendeeId",
                table: "RegistersFor",
                column: "AttendeeId",
                principalTable: "Attendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistersFor_Events_EventId",
                table: "RegistersFor",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SuppliesAt_Events_EventId",
                table: "SuppliesAt",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SuppliesAt_Vendors_VendorId",
                table: "SuppliesAt",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAssignments_Events_EventId",
                table: "TeamAssignments",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAssignments_LockerRooms_LockerRoomId",
                table: "TeamAssignments",
                column: "LockerRoomId",
                principalTable: "LockerRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAssignments_Teams_TeamId",
                table: "TeamAssignments",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorBooths_Venues_VenueId",
                table: "VendorBooths",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
