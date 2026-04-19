using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class AttendeeFormModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, StringLength(20)]
        public string Phone { get; set; } = string.Empty;
    }
}
