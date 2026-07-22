namespace WebApplication2.DTOs.People
{
    public class PersonResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}