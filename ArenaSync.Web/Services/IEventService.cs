using ArenaSync.Web.Models;

namespace ArenaSync.Web.Services
{
    public interface IEventService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event> CreateEventAsync(Event eventEntity);
        Task<Event?> UpdateEventAsync(Event eventEntity);
        Task<bool> DeleteEventAsync(int id);
    }
}