using ArenaSync.Web.Pages.Events;

namespace ArenaSync.Web.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Location { get; set;} = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<SuppliesAt> Supplies { get; set; } = new();
        public List<VendorAssignment> Assignments { get; set; } = new();

    }
}