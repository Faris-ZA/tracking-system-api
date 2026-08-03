using System.Text;
using WebApplication2.DTOs.Reports;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class CsvExportService : ICsvExportService
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<CsvExportService> _logger;

        public CsvExportService(
            IHostEnvironment environment,
            ILogger<CsvExportService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> ExportPeopleLocationReportAsync(
            List<PeopleLocationReportDto> reportData)
        {
            try
            {
                var reportsFolder = Path.Combine(
                    _environment.ContentRootPath,
                    "GeneratedFiles",
                    "PeopleLocationReports");

                Directory.CreateDirectory(reportsFolder);

                var fileName =
                    $"people-location-report-" +
                    $"{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";

                var filePath = Path.Combine(
                    reportsFolder,
                    fileName);

                var csvContent = new StringBuilder();

                csvContent.AppendLine(
                    "PersonId,PersonName,VenueName," +
                    "FloorName,ZoneName,Status,LastSeen");

                foreach (var person in reportData)
                {
                    var lastSeen = person.LastSeen.HasValue
                        ? person.LastSeen.Value
                            .ToString("yyyy-MM-dd HH:mm:ss")
                        : string.Empty;

                    csvContent.AppendLine(
                        $"{person.PersonId}," +
                        $"{EscapeCsvValue(person.PersonName)}," +
                        $"{EscapeCsvValue(person.VenueName)}," +
                        $"{EscapeCsvValue(person.FloorName)}," +
                        $"{EscapeCsvValue(person.ZoneName)}," +
                        $"{EscapeCsvValue(person.Status)}," +
                        $"{EscapeCsvValue(lastSeen)}");
                }

                await File.WriteAllTextAsync(
                    filePath,
                    csvContent.ToString(),
                    Encoding.UTF8);

                _logger.LogInformation(
                    "People location report generated successfully at {FilePath}.",
                    filePath);

                return filePath;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to generate the people location CSV report.");

                throw;
            }
        }

        private static string EscapeCsvValue(
            string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var escapedValue = value.Replace(
                "\"",
                "\"\"");

            if (escapedValue.Contains(',') ||
                escapedValue.Contains('"') ||
                escapedValue.Contains('\n') ||
                escapedValue.Contains('\r'))
            {
                return $"\"{escapedValue}\"";
            }

            return escapedValue;
        }
    }
}