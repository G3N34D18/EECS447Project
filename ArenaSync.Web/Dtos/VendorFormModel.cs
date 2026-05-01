using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class VendorFormModel
    {
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Type { get; set; } = string.Empty;

        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Phone, StringLength(20)]
        public string Phone { get; set; } = string.Empty;
    }
}
