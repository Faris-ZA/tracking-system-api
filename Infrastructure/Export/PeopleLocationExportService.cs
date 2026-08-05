using WebApplication2.DTOs.Reports;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Infrastructure.Export
{
    public class PeopleLocationExportService
        : IPeopleLocationExportService
    {
        private readonly IPeopleLocationReportService
            _reportService;

        private readonly ICsvExportService
            _csvExportService;

        private readonly ILogger<PeopleLocationExportService>
            _logger;

        public PeopleLocationExportService(
            IPeopleLocationReportService reportService,
            ICsvExportService csvExportService,
            ILogger<PeopleLocationExportService> logger)
        {
            _reportService = reportService;
            _csvExportService = csvExportService;
            _logger = logger;
        }

        public async Task<FileExportResultDto>
            ExportAllPeopleAsync()
        {
            try
            {
                _logger.LogInformation(
                    "People location export started.");

                var reportData =
                    await _reportService
                        .GetAllPeopleReportAsync();

                var filePath =
                    await _csvExportService
                        .ExportPeopleLocationReportAsync(
                            reportData);

                var fileContent =
                    await File.ReadAllBytesAsync(filePath);

                var result = new FileExportResultDto
                {
                    Content = fileContent,
                    FileName = Path.GetFileName(filePath),
                    ContentType = "text/csv"
                };

                _logger.LogInformation(
                    "People location export completed successfully.");

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "People location export failed.");

                throw;
            }
        }
    }
}
