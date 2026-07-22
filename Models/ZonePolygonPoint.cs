namespace WebApplication2.Models
{
    public class ZonePolygonPoint
    {
        public int ZoneId { get; set; }

        public int PointIndex { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public Zone Zone { get; set; } = null!;
    }
}