namespace ArenaSync.Web.Models
{
    public class VendorAssignment
    {
        public int VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public int BoothId { get; set; }
        public VendorBooth? Booth { get; set; }
    }
}