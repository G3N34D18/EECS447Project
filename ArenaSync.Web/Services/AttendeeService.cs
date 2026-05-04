using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class AttendeeService : IAttendeeService
    {
        private readonly ApplicationDbContext _context;

        public AttendeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetEventsForAttendeeAsync(int attendeeId)
        {
            return await _context.RegistersFor
                .Where(r => r.AttendeeId == attendeeId)
                .Include(r => r.Event)
                .Select(r => r.Event)
                .OrderBy(e => e.Name)
                .ToListAsync();
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
                    .ThenInclude(r => r.Event)
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

        // UPDATE ATTENDEE 
        public async Task<Attendee?> UpdateAttendeeAsync(Attendee attendee)
        {
            var existingAttendee = await _context.Attendees.FindAsync(attendee.Id);
            if (existingAttendee == null) return null;
            existingAttendee.Name = attendee.Name;
            existingAttendee.Email = attendee.Email;
            existingAttendee.Phone = attendee.Phone;
            await _context.SaveChangesAsync();
            return existingAttendee;
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

        // REGISTER ATTENDEE FOR EVENT 
        public async Task<bool> RegisterAttendeeForEventAsync(int attendeeId, int eventId)
        {
    
            if (!await _context.Attendees.AnyAsync(a => a.Id == attendeeId))
                return false;

            if (!await _context.Events.AnyAsync(e => e.Id == eventId))
            {
                return false;
            }

            bool alreadyRegistered = await _context.RegistersFor
                .AsNoTracking()
                .AnyAsync(r => r.AttendeeId == attendeeId && r.EventId == eventId);

            if (alreadyRegistered)
                return false;


            _context.RegistersFor.Add(new RegistersFor
            {
                AttendeeId = attendeeId,
                EventId = eventId
            });

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

        public async Task<bool> UnregisterAttendeeFromEventAsync(int attendeeId, int eventId)
        {
            var registration = await _context.RegistersFor
                .FirstOrDefaultAsync(r => r.AttendeeId == attendeeId && r.EventId == eventId);

            if (registration == null)
                return false;

            _context.RegistersFor.Remove(registration);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
