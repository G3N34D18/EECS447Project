namespace ArenaSync.Web.Models
{
    public class SuppliesAt
    {
        public int VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}