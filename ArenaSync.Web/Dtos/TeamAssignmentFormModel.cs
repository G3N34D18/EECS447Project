using System.ComponentModel.DataAnnotations;

namespace ArenaSync.Web.Dtos
{
    public class TeamAssignmentFormModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "A team is required.")]
        public int TeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "An event is required.")]
        public int EventId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A locker room is required.")]
        public int LockerId { get; set; }
    }
}
