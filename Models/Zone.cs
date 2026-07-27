namespace WebApplication2.Models
{
    public class Zone
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Floor Floor { get; set; } = null!;

        public int FloorId { get; set; }

        public UpdateStatus UpdateStatus { get; set; } = UpdateStatus.New;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public ICollection<ZonePolygonPoint> PolygonPoints { get; set; }
            = new List<ZonePolygonPoint>();

    }
}