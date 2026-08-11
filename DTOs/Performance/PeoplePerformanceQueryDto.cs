namespace WebApplication2.DTOs.Performance
{
    public class PeoplePerformanceQueryDto
    {
        public int? PersonId { get; set; }

        public string? PersonName { get; set; }

        public string? AssignmentStatus { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 50;
    }
}