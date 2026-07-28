namespace WebApplication2.Models
{
    public class PositionHistory
    {
        public int Id { get; set; }

        public int PeopleId { get; set; }

        public int VenueId { get; set; }

        public int FloorId { get; set; }

        public int ZoneId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public Person Person { get; set; } = null!;

        public Venue Venue { get; set; } = null!;

        public Floor Floor { get; set; } = null!;

        public Zone Zone { get; set; } = null!;
    }
}