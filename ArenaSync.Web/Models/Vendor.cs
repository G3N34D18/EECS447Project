using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Models
{
    public class Vendor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vendor name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vendor type is required.")]
        [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters.