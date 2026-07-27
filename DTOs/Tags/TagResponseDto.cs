namespace WebApplication2.DTOs.Tags
{
    public class TagResponseDto
    {
        public int Id { get; set; }

        public string Label { get; set; } = string.Empty;

        public string Mac { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
