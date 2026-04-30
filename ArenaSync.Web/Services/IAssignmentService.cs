using ArenaSync.Web.Dtos;
using ArenaSync.Web.Models;

namespace ArenaSync.Web.Services
{
    public interface IAssignmentService
    {
        Task<List<TeamAssignment>> GetTeamAssignmentsForEventAsync(int eventId);
        Task<List<VendorAssignment>> GetVendorAssignmentsForEventAsync(int eventId);
        Task<List<LockerRoom>> GetAvailableLockerRoomsForEventAsync(int eventId);
        Task<List<VendorBooth>> GetAvailableVendorBoothsForEventAsync(int eventId);
        Task<AssignmentResult> AssignTeamToLockerAsync(int teamId, int eventId, int lockerId);
        Task<AssignmentResult> AssignVendorToBoothAsync(int vendorId, int eventId, int boothId);
        Task<bool> RemoveTeamLockerAssignmentAsync(int teamId, int eventId, int lockerId);
        Task<bool> RemoveVendorBoothAssignmentAsync(int vendorId, int eventId, int boothId);
        Task<AssignmentConflictDto?> CheckTeamLockerConflictAsync(int eventId, int teamId, int lockerId);
        Task<AssignmentConflictDto?> CheckVendorBoothConflictAsync(int eventId, int vendorId, int boothId);
        Task<List<AssignmentConflictDto>> GetAssignmentConflictsAsync(int eventId);
    }
}
