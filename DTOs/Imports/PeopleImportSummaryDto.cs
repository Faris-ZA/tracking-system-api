namespace WebApplication2.DTOs.Imports
{
    public class PeopleImportSummaryDto
    {
        public int TotalRows { get; set; }

        public int SuccessfulRows { get; set; }

        public int FailedRows { get; set; }

        public List<PeopleImportFailureDto> Failures { get; set; } = new();
    }
}