namespace WebApplication2.DTOs.Floors
{
    public class FloorResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int VenueId { get; set; }

        public int Level { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}