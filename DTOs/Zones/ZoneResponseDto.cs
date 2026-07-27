namespace WebApplication2.DTOs.Zones
{
    public class ZoneResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int FloorId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}