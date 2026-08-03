using WebApplication2.DTOs.Reports;

namespace WebApplication2.Repositories.Interfaces
{
    public interface IPeopleLocationReportRepository
    {
        Task<List<PeopleLocationReportDto>>
            GetPeopleLocationDataAsync();
    }
}