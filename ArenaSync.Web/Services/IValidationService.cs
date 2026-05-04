// -----------------------------------------------------------------------------
// File: IValidationService.cs
// Project: ArenaSync.Web
// Purpose: Batch 4 — Contract for business-rule validation beyond data
//          annotations: date ranges, duplicate assignments, capacity checks.
// -----------------------------------------------------------------------------

namespace ArenaSync.Web.Services
{
    public interface IValidationService
    {
        /// <summary>
        /// Validates that a team can be assigned to an event:
        /// checks the team exists, the event exists, and the team is not
        /// already assigned.
        /// </summary>
        Task<List<string>> ValidateTeamAssignmentAsync(int teamId, int eventId, int lockerId);

        /// <summary>
        /// Validates that a vendor can be assigned to a booth at an event:
        /// checks existence, booth availability, and no duplicate assignment.
        /// </summary>
        Task<List<string>> ValidateVendorBoothAssignmentAsync(int vendorId, int boothId, int eventId);

        /// <summary>
        /// Validates that an attendee registration is valid:
        /// checks for duplicate registration and event capacity.
        /// </summary>
        Task<List<string>> ValidateAttendeeRegistrationAsync(int attendeeId, int eventId);

        /// <summary>
        /// Validates event date/time rules: end must be after start,
        /// and the venue must not already be booked for an overlapping time.
        /// </summary>
        Task<List<string>> ValidateEventDatesAsync(int venueId, DateTime startTime, DateTime endTime, int? excludeEventId = null);
    }
}
