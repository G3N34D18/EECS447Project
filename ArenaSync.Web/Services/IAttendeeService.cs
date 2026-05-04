using ArenaSync.Web.Models;

namespace ArenaSync.Web.Services
{
    public interface IAttendeeService
    {
        Task<List<Attendee>> GetAllAttendeesAsync();
        Task<Attendee?> GetAttendeeByIdAsync(int id);
        Task<bool> CreateAttendeeAsync(Attendee attendee);
        Task<Attendee?> UpdateAttendeeAsync(Attendee attendee);
        Task<bool> DeleteAttendeeAsync(int id);

        // Event registration
        Task<bool> RegisterAttendeeForEventAsync(int attendeeId, int eventId);
        Task<List<Event>> GetEventsForAttendeeAsync(int attendeeId);

        Task<bool> UnregisterAttendeeFromEventAsync(int attendeeId, int eventId);
    }
}
