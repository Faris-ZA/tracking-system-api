namespace WebApplication2.DTOs.Performance
{
    public class PeoplePerformanceQueryDto
    {
        public int? PersonId { get; set; }

        public string? PersonName { get; set; }

        public bool? IsAssigned { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 50;
    }
}
