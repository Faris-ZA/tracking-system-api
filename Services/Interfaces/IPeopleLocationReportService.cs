using WebApplication2.DTOs.Reports;

namespace WebApplication2.Services.Interfaces
{
    public interface IPeopleLocationReportService
    {
        Task<List<PeopleLocationReportDto>>
            GetAllPeopleReportAsync();

        Task<List<PeopleLocationReportDto>>
            GetOfflinePeopleReportAsync();
    }
}