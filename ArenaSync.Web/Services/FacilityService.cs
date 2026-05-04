using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class FacilityService : IFacilityService
    {
        private readonly ApplicationDbContext _context;

        public FacilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LockerRoom>> GetLockerRoomsForVenueAsync(int venueId)
        {
            return await _context.LockerRooms
                .Where(lr => lr.VenueId == venueId)
                .Include(lr => lr.Assignments)
                .OrderBy(lr => lr.RoomNumber)
                .ToListAsync();
        }

        public async Task<LockerRoom?> GetLockerRoomByIdAsync(int id)
        {
            return await _context.LockerRooms
                .Include(lr => lr.Venue)
                .Include(lr => lr.Assignments)
                .FirstOrDefaultAsync(lr => lr.Id == id);
        }

        public async Task<bool> CreateLockerRoomAsync(LockerRoom lockerRoom)
        {
            var venueExists = await _context.Venues.AnyAsync(v => v.Id == lockerRoom.VenueId);
            var duplicate = await _context.LockerRooms
                .AnyAsync(lr => lr.VenueId == lockerRoom.VenueId && lr.RoomNumber == lockerRoom.RoomNumber);

            if (!venueExists || duplicate)
            {
                return false;
            }

            _context.LockerRooms.Add(lockerRoom);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLockerRoomAsync(LockerRoom lockerRoom)
        {
            var existingLockerRoom = await _context.LockerRooms.FindAsync(lockerRoom.Id);
            if (existingLockerRoom is null)
            {
                return false;
            }

            var duplicate = await _context.LockerRooms.AnyAsync(lr =>
                lr.Id != lockerRoom.Id
                && lr.VenueId == existingLockerRoom.VenueId
                && lr.RoomNumber == lockerRoom.RoomNumber);

            if (duplicate)
            {
                return false;
            }

            existingLockerRoom.RoomNumber = lockerRoom.RoomNumber;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLockerRoomAsync(int id)
        {
            var lockerRoom = await _context.LockerRooms.FindAsync(id);
            if (lockerRoom is null)
            {
                return false;
            }

            var hasAssignments = await _context.TeamAssignments.AnyAsync(ta => ta.LockerId == id);
            if (hasAssignments)
            {
                return false;
            }

            _context.LockerRooms.Remove(lockerRoom);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VendorBooth>> GetVendorBoothsForVenueAsync(int venueId)
        {
            return await _context.VendorBooths
                .Where(b => b.VenueId == venueId)
                .Include(b => b.VendorAssignments)
                .OrderBy(b => b.BoothNumber)
                .ToListAsync();
        }

        public async Task<VendorBooth?> GetVendorBoothByIdAsync(int id)
        {
            return await _context.VendorBooths
                .Include(b => b.Venue)
                .Include(b => b.VendorAssignments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> CreateVendorBoothAsync(VendorBooth booth)
        {
            var venueExists = await _context.Venues.AnyAsync(v => v.Id == booth.VenueId);
            var duplicate = await _context.VendorBooths
                .AnyAsync(b => b.VenueId == booth.VenueId && b.BoothNumber == booth.BoothNumber);

            if (!venueExists || duplicate)
            {
                return false;
            }

            _context.VendorBooths.Add(booth);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVendorBoothAsync(VendorBooth booth)
        {
            var existingBooth = await _context.VendorBooths.FindAsync(booth.Id);
            if (existingBooth is null)
            {
                return false;
            }

            var duplicate = await _context.VendorBooths.AnyAsync(b =>
                b.Id != booth.Id
                && b.VenueId == existingBooth.VenueId
                && b.BoothNumber == booth.BoothNumber);

            if (duplicate)
            {
                return false;
            }

            existingBooth.BoothNumber = booth.BoothNumber;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVendorBoothAsync(int id)
        {
            var booth = await _context.VendorBooths.FindAsync(id);
            if (booth is null)
            {
                return false;
            }

            var hasAssignments = await _context.VendorAssignments.AnyAsync(va => va.BoothId == id);
            if (hasAssignments)
            {
                return false;
            }

            _context.VendorBooths.Remove(booth);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
