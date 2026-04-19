using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class VenueService : IVenueService
    {
        public readonly ApplicationDbContext _context;

        public VenueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Venue>> GetAllVenuesAsync()
        {
            return await _context.Venues.OrderBy(v => v.Name).ToListAsync();
        }

        public async Task<Venue?> GetVenueByIdAsync(int id)
        {
            return await _context.Venues.Include(v => v.Events).FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Venue> CreateVenueAsync(Venue venueEntity)
        {
            _context.Venues.Add(venueEntity);
            await _context.SaveChangesAsync();
            return venueEntity;
        }

        public async Task<Venue?> UpdateVenueAsync(Venue venueEntity)
        {
            var existingVenue = await _context.Venues.FindAsync(venueEntity.Id);
            if (existingVenue == null) return null;
            existingVenue.Name = venueEntity.Name;
            existingVenue.Address = venueEntity.Address;
            existingVenue.Capacity = venueEntity.Capacity;
            await _context.SaveChangesAsync();
            return existingVenue;
        }

        public async Task<bool> DeleteVenueAsync(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return false;
            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}