namespace WebApplication2.DTOs.Venues
{
    public class VenueResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}