using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class VendorFormModel
    {
        [Required(ErrorMessage = "Vendor name is required."), StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vendor type is required."), StringLength(100, ErrorMessage = "Type cannot exceed 100 characters.")]
        public string Type { get; set; } = string.Empty;

     