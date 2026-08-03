namespace WebApplication2.DTOs.Reports
{
    public class PeopleLocationReportDto
    {
        public int PersonId { get; set; }

        public string PersonName { get; set; } = string.Empty;

        public string? VenueName { get; set; }

        public string? FloorName { get; set; }

        public string? ZoneName { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? LastSeen { get; set; }

    }
}
