using ArenaSync.Web.Models;

namespace ArenaSync.Web.Services
{
    public interface IFacilityService
    {
        Task<List<LockerRoom>> GetLockerRoomsForVenueAsync(int venueId);
        Task<LockerRoom?> GetLockerRoomByIdAsync(int id);
        Task<bool> CreateLockerRoomAsync(LockerRoom lockerRoom);
        Task<bool> UpdateLockerRoomAsync(LockerRoom lockerRoom);
        Task<bool> DeleteLockerRoomAsync(int id);

        Task<List<VendorBooth>> GetVendorBoothsForVenueAsync(int venueId);
        Task<VendorBooth?> GetVendorBoothByIdAsync(int id);
        Task<bool> CreateVendorBoothAsync(VendorBooth booth);
        Task<bool> UpdateVendorBoothAsync(VendorBooth booth);
        Task<bool> DeleteVendorBoothAsync(int id);
    }
}
