namespace WebApplication2.DTOs.Performance
{
    public class TagPerformanceQueryDto
    {
        public int? TagId { get; set; }

        public string? TagLabel { get; set; }

        public string? AssignmentStatus { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 50;
    }
}