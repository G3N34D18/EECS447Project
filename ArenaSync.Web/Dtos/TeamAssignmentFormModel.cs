using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class TeamAssignmentFormModel
    {
        [Required]
        public int TeamId { get; set; }

        [Required]
        public int EventId { get; set; }
    }
}
