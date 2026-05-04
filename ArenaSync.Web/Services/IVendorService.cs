using ArenaSync.Web.Models;

namespace ArenaSync.Web.Services
{
    public interface IVendorService
    {
        Task<List<Vendor>> GetAllVendorsAsync();
        Task<Vendor?> GetVendorByIdAsync(int id);
        Task<bool> CreateVendorAsync(Vendor vendor);
        Task<Vendor?> UpdateVendorAsync(Vendor vendor);
        Task<bool> DeleteVendorAsync(int id);
        Task<List<Vendor>> GetVendorsForEventAsync(int eventId);
        Task<List<Vendor>> GetAvailableVendorsForEventAsync(int eventId);
        Task<bool> AddVendorToEventAsync(int vendorId, int eventId);
        Task<bool> RemoveVendorFromEventAsync(int vendorId, int eventId);
    }
}
