namespace WebApplication2.Models
{
    public class Floor
    {
    
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public int VenueId { get; set; }

            public int Level { get; set; }

            public UpdateStatus UpdateStatus { get; set; } = UpdateStatus.New;

            public DateTime CreateDate { get; set; } = DateTime.Now;

            public DateTime LastUpdate { get; set; } = DateTime.Now;

            public Venue Venue { get; set; } = null!;

            public ICollection<Zone> Zones { get; set; } = new List<Zone>();


    }
}