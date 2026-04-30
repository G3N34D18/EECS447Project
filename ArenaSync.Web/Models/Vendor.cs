namespace ArenaSync.Web.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Location { get; set;} = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<SuppliesAt> SuppliesAt { get; set; } = new();
        public List<VendorAssignment> VendorAssignments { get; set; } = new();

    }
}
