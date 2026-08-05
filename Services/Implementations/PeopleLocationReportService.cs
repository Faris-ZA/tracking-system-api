using WebApplication2.DTOs.Reports;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class PeopleLocationReportService
        : IPeopleLocationReportService
    {
        private const int LiveThresholdMinutes = 5;

        private readonly IPeopleLocationReportRepository
            _reportRepository;

        public PeopleLocationReportService(
            IPeopleLocationReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<PeopleLocationReportDto>>
            GetAllPeopleReportAsync()
        {
            var reportData =
                await _reportRepository
                    .GetPeopleLocationDataAsync();

            var liveThreshold =
                DateTime.UtcNow.AddMinutes(
                    -LiveThresholdMinutes);

            foreach (var person in reportData)
            {
                person.IsLive =
                    person.LastSeen.HasValue &&
                    person.LastSeen.Value >= liveThreshold;
            }

            return reportData;
        }

        public async Task<List<PeopleLocationReportDto>>
            GetOfflinePeopleReportAsync()
        {
            var reportData =
                await GetAllPeopleReportAsync();

            return reportData
                .Where(person =>
                    person.IsLive)
                .ToList();
        }
    }
}