namespace AnalyticsService.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
