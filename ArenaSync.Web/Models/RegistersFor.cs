namespace ArenaSync.Web.Models
{
    public class RegistersFor
    {
        public int AttendeeId { get; set; }
        public Attendee? Attendee { get; set; } 
        public int EventId { get; set; }
        public Event? Event { get; set; }    
    }
}