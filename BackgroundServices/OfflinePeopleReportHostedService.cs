using Microsoft.Extensions.Options;
using WebApplication2.Configuration;
using WebApplication2.Services.Implementations;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.BackgroundServices
{
    public class OfflinePeopleReportHostedService
        : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<OfflinePeopleReportHostedService>
            _logger;

        private readonly TimeSpan _interval;

        public OfflinePeopleReportHostedService(
            IServiceScopeFactory scopeFactory,
            IOptions<BackgroundJobSettings> options,
            ILogger<OfflinePeopleReportHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var intervalMinutes =
                options.Value.IntervalMinutes > 0
                    ? options.Value.IntervalMinutes
                    : 60;

            _interval =
                TimeSpan.FromMinutes(intervalMinutes);
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Offline people report hosted service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await GenerateOfflineReportAsync(
                            stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "An error occurred while processing the scheduled offline people report.");
                }

                try
                {
                    await Task.Delay(
                        _interval,
                        stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation(
                "Offline people report hosted service stopped.");
        }

        private async Task GenerateOfflineReportAsync(
            CancellationToken cancellationToken)
        {
            using var scope =
                _scopeFactory.CreateScope();

            var reportService =
                scope.ServiceProvider
                    .GetRequiredService<
                        IPeopleLocationReportService>();

            var csvExportService =
                scope.ServiceProvider
                    .GetRequiredService<
                        ICsvExportService>();

            var emailService =
                scope.ServiceProvider
                    .GetRequiredService<
                        IEmailService>();

            _logger.LogInformation(
                "Scheduled offline people report processing started.");

            var offlinePeople =
                await reportService
                    .GetOfflinePeopleReportAsync();

            var filePath =
                await csvExportService
                    .ExportPeopleLocationReportAsync(
                        offlinePeople);

            await emailService
                .SendOfflinePeopleReportAsync(
                    filePath,
                    offlinePeople.Count,
                    cancellationToken);

            _logger.LogInformation(
                "Scheduled offline people report completed. " +
                "Offline people count: {OfflinePeopleCount}. " +
                "File: {FilePath}",
                offlinePeople.Count,
                filePath);
        }
    }
}