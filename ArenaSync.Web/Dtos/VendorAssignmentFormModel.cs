using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class VendorAssignmentFormModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "A vendor is required.")]
        public int VendorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "An event is required.")]
        public int EventId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A booth is required.")]
        public int BoothId { get; set; }
    }
}
