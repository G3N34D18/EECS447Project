using ArenaSync.Web.Pages.Venues;

namespace ArenaSync.Web.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public List<LockerRoom> LockerRooms { get; set; } = new();
        public List<VendorBooth> VendorBooths { get; set; } = new();
        public List<Event> Events { get; set; } = new();
    }
}