using ArenaSync.Web.Models;

namespace ArenaSync.Web.Services
{
    public interface IVenueService
    {
        Task<List<Venue>> GetAllVenuesAsync();
        Task<Venue?> GetVenueByIdAsync(int id);
        Task<Venue> CreateVenueAsync(Venue venueEntity);
        Task<Venue?> UpdateVenueAsync(Venue venueEntity);
        Task<bool> DeleteVenueAsync(int id);
    }
}