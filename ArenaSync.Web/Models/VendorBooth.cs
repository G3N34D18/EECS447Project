using ArenaSync.Web.Pages.Events;

namespace ArenaSync.Web.Models
{
    public class VendorBooth
    {
        public int Id { get; set; }
        public int BoothNumber { get; set; }
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }
        public List<VendorAssignment> VendorAssignments { get; set; } = new();   
    }
}