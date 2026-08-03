using WebApplication2.DTOs.Reports;

namespace WebApplication2.Services.Interfaces
{
    public interface ICsvExportService
    {
        Task<string> ExportPeopleLocationReportAsync(
            List<PeopleLocationReportDto> reportData);
    }
}