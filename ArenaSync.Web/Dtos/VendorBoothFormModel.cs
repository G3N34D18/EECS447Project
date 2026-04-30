using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class VendorBoothFormModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Booth number must be greater than 0.")]
        public int BoothNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A venue is required.")]
        public int VenueId { get; set; }
    }
}
