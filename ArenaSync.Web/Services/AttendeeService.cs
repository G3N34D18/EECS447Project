using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class AttendeeService
    {
        private readonly ApplicationDbContext _context;

        public AttendeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL ATTENDEES
        public async Task<List<Attendee>> GetAllAttendeesAsync()
        {
            return await _context.Attendees
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        // GET ATTENDEE BY ID
        public async Task<Attendee?> GetAttendeeByIdAsync(int id)
        {
            return await _context.Attendees
                .Include(a => a.Registrations)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // CREATE ATTENDEE (duplicate email check)
        public async Task<bool> CreateAttendeeAsync(Attendee attendee)
        {
            bool exists = await _context.Attendees
                .AnyAsync(a => a.Email.ToLower() == attendee.Email.ToLower());

            if (exists)
                return false;

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();
            return true;
        }

        // UPDATE ATTENDEE (duplicate email check)
        public async Task<bool> UpdateAttendeeAsync(Attendee attendee)
        {
            bool exists = await _context.Attendees
                .AnyAsync(a => a.Id != attendee.Id &&
                               a.Email.ToLower() == attendee.Email.ToLower());

            if (exists)
                return false;

            _context.Attendees.Update(attendee);
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE ATTENDEE
        public async Task<bool> DeleteAttendeeAsync(int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
                return false;

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();
            return true;
        }

        // REGISTER ATTENDEE FOR EVENT (duplicate check)
        public async Task<bool> RegisterAttendeeForEventAsync(int attendeeId, int eventId)
        {
            // Check if already registered
            bool exists = await _context.RegistersFor
                .AnyAsync(r => r.AttendeeId == attendeeId && r.EventId == eventId);

            if (exists)
                return false;

            // ensure attendee exists
            bool attendeeExists = await _context.Attendees.AnyAsync(a => a.Id == attendeeId);
            if (!attendeeExists)
                return false;

            // ensure event exists
            bool eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists)
                return false;

            var registration = new RegistersFor
            {
                AttendeeId = attendeeId,
                EventId = eventId
            };

            _context.RegistersFor.Add(registration);
            await _context.SaveChangesAsync();
            return true;
        }

        // GET ATTENDEES FOR EVENT
        public async Task<List<Attendee>> GetAttendeesForEventAsync(int eventId)
        {
            return await _context.RegistersFor
                .Where(r => r.EventId == eventId)
                .Include(r => r.Attendee)
                .Select(r => r.Attendee)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
    }
}
